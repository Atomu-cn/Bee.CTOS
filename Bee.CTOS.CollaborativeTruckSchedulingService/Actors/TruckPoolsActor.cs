using Dapr.Actors;
using Dapr.Actors.Runtime;
using Newtonsoft.Json;
using Bee.CTOS.CollaborativeTruckSchedulingService.Configs;
using Bee.CTOS.CollaborativeTruckSchedulingService.Models;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Actors;

/// <summary>
/// 集卡池
/// </summary>
/// ID: $"{{\"TerminalNo\":\"{terminalNo}\",\"TruckPoolsNo\":\"{truckPoolsNo}\"}}"
public class TruckPoolsActor : Actor, IRemindable, ITruckPoolsActor
{
    public TruckPoolsActor(ActorHost host)
        : base(host)
    {
        dynamic? truckPools = JsonConvert.DeserializeObject<dynamic>(this.Id.ToString());
        if (truckPools == null)
            throw new NotSupportedException($"本{this.GetType().FullName}不支持用'{this.Id}'格式构造TruckPools对象!");

        _terminalNo = truckPools.TerminalNo;
        _truckPoolsNo = truckPools.TruckPoolsNo;
        _truckPools = TruckPools.FetchRoot(p => p.TerminalNo == _terminalNo && p.TruckPoolsNo == _truckPoolsNo);
    }

    #region 属性

    private readonly string _terminalNo;
    private readonly string _truckPoolsNo;
    private TruckPools? _truckPools;
    private const string _truckPoolsReminderName = "TruckPoolsReminder";
    private const string _truckPoolsTimerName = "TruckPoolsTimer";
    private ActorTimer? _timer;

    #endregion

    #region 方法

    private async Task RegisterReminderAsync()
    {
        await this.RegisterReminderAsync(_truckPoolsReminderName, null, TimeSpan.FromSeconds(0), ActorConfig.ActorIdleTimeout);
    }

    private async Task UnRegisterReminderAsync()
    {
        await this.UnregisterReminderAsync(_truckPoolsReminderName);
        if (_timer != null)
        {
            await this.UnregisterTimerAsync(_timer);
            _timer = null;
        }
    }

    async Task IRemindable.ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
    {
        if (_timer == null && _truckPools != null && !_truckPools.Invalided)
            _timer = await this.RegisterTimerAsync(_truckPoolsTimerName, nameof(OnTimerCallBack), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    private async Task OnTimerCallBack(byte[] data)
    {
        List<Task<TruckCarryingTask>> tasks = new List<Task<TruckCarryingTask>>(_truckPools.Trucks.Length);
        foreach (TruckPoolsTruck item in _truckPools.Trucks)
            tasks.Add(Task.Run(() => this.ProxyFactory.CreateActorProxy<ITruckActor>(new ActorId(item.TruckNo), nameof(TruckActor)).
                NewTaskAsync(task, (Models.TruckLoadingPosition)Int32.Parse(msg.PlanLoadingPosition))));
        await Task.WhenAll(tasks);
        foreach (Task<TruckCarryingTask> item in tasks)
        {

        }
    }

    #region API

    /// <summary>
    /// Put
    /// </summary>
    public async Task PutAsync(string[] truckNos)
    {
        if (_truckPools == null)
        {
            _truckPools = TruckPools.Create(_terminalNo, _truckPoolsNo, truckNos);
            await RegisterReminderAsync();
        }
        else
            _truckPools.ReplaceTrucks(truckNos);
    }

    /// <summary>
    /// 作废
    /// </summary>
    public async Task InvalidAsync()
    {
        if (_truckPools == null)
            throw new NotSupportedException($"本码头'{_terminalNo}'集卡池'{_truckPoolsNo}'还未创建, 无法作废!");

        _truckPools.Invalid();
        await UnRegisterReminderAsync();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public async Task ResumeAsync()
    {
        if (_truckPools == null)
            throw new NotSupportedException($"本码头'{_terminalNo}'集卡池'{_truckPoolsNo}'还未创建, 无法恢复!");

        _truckPools.Resume();
        await RegisterReminderAsync();
    }

    /// <summary>
    /// 处理新的运输任务
    /// </summary>
    public Task NewCarryingTaskAsync(Events.CarryingTask msg)
    {
        if (_truckPools == null)
            throw new NotSupportedException($"本码头'{_terminalNo}'集卡池'{_truckPoolsNo}'还未创建, 无法处理新的运输任务!");
        if (_truckPools.Invalided)
            throw new InvalidOperationException($"本码头'{_terminalNo}'集卡池'{_truckPoolsNo}'已于{_truckPools.InvalidedChangeTime}禁用, 无法处理新的运输任务!");

        _truckPools.AddTask(CarryingTask.Create(msg.TerminalNo, msg.TruckPoolsNo,
            msg.TaskId, (CarryingTaskType)Int32.Parse(msg.TaskType), msg.TaskPriority,
            !String.IsNullOrEmpty(msg.PlanLoadingPosition) ? (TruckLoadingPosition)Int32.Parse(msg.PlanLoadingPosition) : null,
            msg.PlanContainerNumber, msg.PlanIsBigSize,
            msg.LoadLocation, msg.LoadQueueNo, msg.loadPlanTime, msg.LoadCraneNo,
            msg.UnloadLocation, msg.UnloadQueueNo, msg.unloadPlanTime, msg.UnloadCraneNo, msg.NeedTwistLock,
            !String.IsNullOrEmpty(msg.QuayCraneProcess) ? (QuayCraneProcess)Int32.Parse(msg.QuayCraneProcess) : null));
        return Task.CompletedTask;
    }

    #endregion

    #endregion
}