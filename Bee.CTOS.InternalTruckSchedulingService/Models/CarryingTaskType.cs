namespace Bee.CTOS.InternalTruckSchedulingService.Models;

/// <summary>
/// 运输任务类型
/// </summary>
public enum CarryingTaskType
{
    /// <summary>
    /// 装船
    /// </summary>
    Shipment,

    /// <summary>
    /// 卸船
    /// </summary>
    Discharge,
    
    /// <summary>
    /// 转运
    /// </summary>
    Transship,

    /// <summary>
    /// 转堆
    /// </summary>
    Shift,
    
}