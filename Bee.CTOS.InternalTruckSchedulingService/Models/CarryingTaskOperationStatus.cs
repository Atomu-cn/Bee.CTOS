namespace Bee.CTOS.InternalTruckSchedulingService.Models;

/// <summary>
/// 运输作业状态
/// </summary>
public enum CarryingTaskOperationStatus
{
    /// <summary>
    /// 未启动
    /// </summary>
    UnStart,

    /// <summary>
    /// 去锁钮站
    /// </summary>
    ToTwistLockStop,

    /// <summary>
    /// 在锁钮站
    /// </summary>
    InTwistLockStop,

    /// <summary>
    /// 完成锁钮作业
    /// </summary>
    TwistLockCompleted,

    /// <summary>
    /// 去装卸
    /// </summary>
    ToLocation,

    /// <summary>
    /// 已到位
    /// </summary>
    OnLocation,

    /// <summary>
    /// 对位中
    /// </summary>
    LocationAligning,

    /// <summary>
    /// 已对位
    /// </summary>
    LocationAligned,

    /// <summary>
    /// 已装卸
    /// </summary>
    LoadUnloaded,
}