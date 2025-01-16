using Dapr.Actors;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图节点
/// </summary>
public interface ITopologicalMapNodeActor : IActor
{
    /// <summary>
    /// 重置
    /// </summary>
    Task ResetAsync();
}