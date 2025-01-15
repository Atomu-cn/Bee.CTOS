using Dapr.Actors;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 集卡池
/// </summary>
public interface ITruckPoolsActor : IActor
{
    /// <summary>
    /// Put
    /// </summary>
    Task PutAsync(string[] truckNos);

    /// <summary>
    /// 作废
    /// </summary>
    Task InvalidAsync();

    /// <summary>
    /// 恢复
    /// </summary>
    Task ResumeAsync();

    /// <summary>
    /// 新的运输任务
    /// </summary>
    Task NewCarryingTaskAsync(Events.CarryingTask msg);
}