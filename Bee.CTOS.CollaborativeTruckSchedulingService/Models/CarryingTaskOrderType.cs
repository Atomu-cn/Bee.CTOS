namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models;

/// <summary>
/// 运输任务指令类型
/// </summary>
public enum CarryingTaskOrderType
{
    /// <summary>
    /// 装载（载到集卡）
    /// </summary>
    Load,

    /// <summary>
    /// 卸载（卸下集卡）
    /// </summary>
    Unload,
}