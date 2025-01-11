using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Phenix.Core.Log;

namespace Phenix.Core.Net.Http
{
    /// <summary>
    /// 异常处理中间件
    /// 
    /// 拦截异常并转译成 context.Response.StatusCode
    /// 异常详情见报文体：
    /// System.ArgumentException 转译为 400 BadRequest
    /// System.Security.Authentication.AuthenticationException 转译为 401 Unauthorized
    /// System.Security.SecurityException 转译为 403 Forbidden
    /// System.InvalidOperationException 转译为 404 NotFound
    /// System.ComponentModel.DataAnnotations.ValidationException 转译为 409 Conflict
    /// System.NotSupportedException/System.NotImplementedException 转译为 501 NotImplemented
    /// 除以上之外的异常都转译为 500 InternalServerError
    /// </summary>
    public sealed class ExceptionHandlerMiddleware
    {
        /// <summary>
        /// 异常处理中间件
        /// </summary>
        /// <param name="next">下一个中间件</param>
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #region 属性

        private readonly RequestDelegate _next;

        #endregion

        #region 方法

        /// <summary>
        /// 动态执行
        /// </summary>
        /// <param name="context">上下文</param>
        public async Task InvokeAsync(HttpContext context)
        {
            DateTime dateTime = DateTime.Now;
            try
            {
                await _next.Invoke(context);

                if (AppRun.Debugging || DateTime.Now.Subtract(dateTime).Seconds > 3)
                    LogHelper.Debug("{@Context} consume time {@TotalMilliseconds} ms",
                        new
                        {
                            context.Request.Path,
                            context.Request.QueryString,
                            context.Request.Method,
                            context.Request.ContentType,
                            context.Response.StatusCode,
                        },
                        DateTime.Now.Subtract(dateTime).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "{@Context}",
                    new
                    {
                        context.Request.Path,
                        context.Request.QueryString,
                        context.Request.Method,
                        context.Request.ContentType,
                        context.Response.StatusCode,
                    });
                await context.Response.PackAsync(ex);
            }
        }

        #endregion
    }
}