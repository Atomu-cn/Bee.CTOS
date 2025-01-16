using Dapr.Actors;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图节点
/// </summary>
public interface ITopologicalMapNodeActor : IActor
{
    /// <summary>
    /// 重置自动运行
    /// </summary>
    Task ResetAutoRunAsync();
}