using System.Runtime.Serialization;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Events;

/// <summary>
/// 运输任务
/// </summary>
[DataContract]
public readonly record struct CarryingTask(
    [property: DataMember] string TerminalNo, //码头编号
    [property: DataMember] string TruckPoolsNo, //集卡池号
    [property: DataMember] long TaskId, //任务ID
    [property: DataMember] string TaskType, //任务类型（可与 CarryingTaskType 常数值互转）
    [property: DataMember] int TaskPriority, //任务优先级
    [property: DataMember] string? PlanLoadingPosition, //计划载箱位置（可与 TruckLoadingPosition 常数值互转）
    [property: DataMember] string PlanContainerNumber, //计划箱号
    [property: DataMember] bool PlanIsBigSize, //计划是大箱
    [property: DataMember] string LoadLocation, //装载位置（地图标记位置）
    [property: DataMember] long LoadQueueNo, //装载排队序号（同一装载位置需按序号排队，同号可交换）
    [property: DataMember] DateTime loadPlanTime, //装载计划时间
    [property: DataMember] string? LoadCraneNo, //装载（载到集卡）机械号
    [property: DataMember] string UnloadLocation, //卸载位置（地图标记位置）
    [property: DataMember] long UnloadQueueNo, //卸载排队序号（同一装载位置需按序号排队，同号可交换）
    [property: DataMember] DateTime unloadPlanTime, //卸载计划时间
    [property: DataMember] string? UnloadCraneNo, //卸载（卸下集卡）机械号
    [property: DataMember] bool NeedTwistLock, //是否需要扭锁（过锁钮站）
    [property: DataMember] string? QuayCraneProcess //岸桥工艺（可与 QuayCraneProcess 常数值互转）
);