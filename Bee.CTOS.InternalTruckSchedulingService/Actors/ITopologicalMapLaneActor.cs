using Dapr.Actors;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图车道（单向）
/// </summary>
public interface ITopologicalMapLaneActor : IActor
{
    /// <summary>
    /// 重置
    /// </summary>
    Task ResetAsync();
}