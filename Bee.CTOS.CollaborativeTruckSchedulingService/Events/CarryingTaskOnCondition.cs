using System.Runtime.Serialization;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Events;

/// <summary>
/// 运输任务条件
/// </summary>
[DataContract]
public readonly record struct CarryingTaskOnCondition(
    [property: DataMember] double TravelDistance //行驶距离(含完成当前任务剩余行驶距离+完成当前任务到接到任务行驶距离)
);
