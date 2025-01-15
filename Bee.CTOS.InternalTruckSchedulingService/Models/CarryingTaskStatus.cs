namespace Bee.CTOS.InternalTruckSchedulingService.Models;

/// <summary>
/// 运输任务状态
/// </summary>
public enum CarryingTaskStatus
{
    /// <summary>
    /// 未启动
    /// </summary>
    UnStart,

    /// <summary>
    /// 执行中
    /// </summary>
    Executing,

    /// <summary>
    /// 已装载（载到集卡）相当于：LoadOrder.Completed && !UnloadOrder.Completed
    /// </summary>
    Loaded,

    /// <summary>
    /// 已卸载（卸下集卡）相当于：LoadOrder.Completed && UnloadOrder.Completed
    /// </summary>
    Unloaded,
}