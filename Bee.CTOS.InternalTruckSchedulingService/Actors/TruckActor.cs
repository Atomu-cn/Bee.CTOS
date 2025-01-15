using Dapr.Actors.Runtime;
using Bee.CTOS.InternalTruckSchedulingService.Models;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 集卡
/// ID: TruckNo
/// </summary>
public class TruckActor : Actor, ITruckActor
{
    public TruckActor(ActorHost host)
        : base(host)
    {
        _truckNo = this.Id.ToString();
        _truck = Truck.FetchRoot(p => p.TruckNo == _truckNo);
    }

    #region 属性

    private readonly string _truckNo;
    private Truck? _truck;

    #endregion

    #region 方法

    #region API

    /// <summary>
    /// Put
    /// </summary>
    public Task PutAsync(TruckDriveType driveType)
    {
        if (_truck == null)
            _truck = Truck.Create(_truckNo, driveType);
        else
            _truck.ChangeDriveType(driveType);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 更改健康状态
    /// </summary>
    public Task ChangeHealthStatusAsync(TruckHealthStatus healthStatus)
    {
        if (_truck == null)
            throw new NotSupportedException($"本'{_truckNo}'集卡还未创建, 无法更改健康状态!");

        _truck.ChangeHealthStatus(healthStatus);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 接受新任务
    /// </summary>
    public Task NewTaskAsync(CarryingTask task)
    {
        if (_truck == null)
            throw new NotSupportedException($"本'{_truckNo}'集卡还未创建, 无法接受新任务!");

        _truck.NewTask(task);
        return Task.CompletedTask;
    }

    #endregion

    #endregion
}