using Dapr.Actors;
using Bee.CTOS.InternalTruckSchedulingService.Models;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 集卡
/// </summary>
public interface ITruckActor : IActor
{
    /// <summary>
    /// Put
    /// </summary>
    Task PutAsync(TruckDriveType driveType);

    /// <summary>
    /// 更改健康状态
    /// </summary>
    Task ChangeHealthStatusAsync(TruckHealthStatus healthStatus);

    /// <summary>
    /// 接受新任务
    /// </summary>
    Task NewTaskAsync(CarryingTask task);
}