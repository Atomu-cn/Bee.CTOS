using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Phenix.Core;
using Phenix.Core.Log;
using Phenix.Core.Reflection;
using Bee.CTOS.CollaborativeTruckSchedulingService.Actors;
using Bee.CTOS.CollaborativeTruckSchedulingService.Configs;
using Bee.CTOS.CollaborativeTruckSchedulingService.DomainServices;
using Serilog;
using Serilog.Events;

namespace Bee.CTOS.CollaborativeTruckSchedulingService;

public static class Program
{
    public static void Main(string[] args)
    {
        //兼容Linux（CentOS）环境
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("zh-CN", true)
        {
            DateTimeFormat = { ShortDatePattern = "yyyy-MM-dd", FullDateTimePattern = "yyyy-MM-dd HH:mm:ss", LongTimePattern = "HH:mm:ss" }
        };

#if DEBUG
        Log.Logger = LogHelper.InitializeConfiguration()
            .CreateLogger();
        AppRun.Debugging = true;
#else
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Exceptionless(
                restrictedToMinimumLevel: AppRun.Debugging ? LogEventLevel.Debug : LogEventLevel.Information, //捕获的最小日志级别
                includeProperties: true, //包含Serilog属性
                serverUrl: LogConfig.ServerUrl,
                apiKey: LogConfig.ApiKey)
            .CreateLogger();
        if (AppRun.Debugging)
            LogHelper.Warning("please set this Phenix.Core.AppRun.Debugging parameter to false in the production environment");
#endif

        try
        {
            using (WebApplication app = CreateWebApplication(args))
            {
                // let's go!
                app.Run("http://localhost:6000");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Fatal(ex, "service terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static WebApplication CreateWebApplication(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Host.UseContentRoot(Phenix.Core.AppRun.BaseDirectory);

        //Logging
        builder.Host.ConfigureLogging(logging => logging.ClearProviders());
        builder.Host.UseSerilog(dispose: true);

        //DomainServices
        builder.Services.AddSingleton<IPathPlanningService, PathPlanningService>();
        builder.Services.AddSingleton<ITaskDispatchService, TaskDispatchService>();
        builder.Services.AddSingleton<ITrafficManagementService, TrafficManagementService>();
        builder.Services.AddSingleton<ITrajectoryPlanningService, TrajectoryPlanningService>();

        //DaprClient
        string daprHttpPort = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3600";
        string daprGrpcPort = Environment.GetEnvironmentVariable("DAPR_GRPC_PORT") ?? "60000";
        builder.Services.AddDaprClient(clientBuilder => clientBuilder
            .UseJsonSerializationOptions(
                new JsonSerializerOptions()
                {
                    ReferenceHandler = ReferenceHandler.Preserve //在序列化和反序列化的时候保留引用并处理循环引用
                })
            .UseHttpEndpoint($"http://localhost:{daprHttpPort}")
            .UseGrpcEndpoint($"http://localhost:{daprGrpcPort}"));

        //Dapr
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = Utilities.JsonDateFormatString;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.Formatting = Formatting.None;
                options.UseMemberCasing();
            })
            .AddDapr(clientBuilder =>
                clientBuilder.UseJsonSerializationOptions(
                    new JsonSerializerOptions()
                    {
                        ReferenceHandler = ReferenceHandler.Preserve //在序列化和反序列化的时候保留引用并处理循环引用
                    }));

        //Actors
        builder.Services.AddActors(options =>
        {
            options.Actors.RegisterActor<TruckPoolsActor>();
            options.Actors.RegisterActor<TruckActor>();
            options.Actors.RegisterActor<TopologicalMapActor>();
            options.ActorIdleTimeout = ActorConfig.ActorIdleTimeout;
            options.ActorScanInterval = ActorConfig.ActorScanInterval;
            options.DrainOngoingCallTimeout = ActorConfig.DrainOngoingCallTimeout;
            options.DrainRebalancedActors = ActorConfig.DrainRebalancedActors;
            options.RemindersStoragePartitions = ActorConfig.RemindersStoragePartitions;
        });

        WebApplication result = builder.Build();
        //configure middleware
        result.UseMiddleware<Phenix.Core.Net.Http.ExceptionHandlerMiddleware>(); //使用异常处理中间件
        //configure web-app
        if (result.Environment.IsDevelopment())
            result.UseDeveloperExceptionPage();
        result.UseRouting();
        result.UseCloudEvents(); //CloudEvents是一种标准化的消息传递格式，提供了一种通用的方法来描述跨平台的事件信息，用于Dapr的pubsub功能。 
        result.UseAuthorization();

        //configure routing
        result.MapSubscribeHandler(); //添加一个Dapr订阅端点，这个端点将响应/dapr/subscribe上的请求，自动找到所有用Topic属性装饰的WebAPI操作方法，并指示Dapr为它们创建订阅。
        result.MapActorsHandlers(); //Register actors handlers that interface with the Dapr runtime.
        result.MapControllers(); //映射用于 REST API 的属性路由控制器

        return result;
    }
}