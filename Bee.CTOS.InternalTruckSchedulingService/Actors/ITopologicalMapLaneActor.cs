using Dapr.Actors;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图车道（单向）
/// </summary>
public interface ITopologicalMapLaneActor : IActor
{
    /// <summary>
    /// 关停
    /// </summary>
    Task ShutdownAsync();

    /// <summary>
    /// 重置
    /// </summary>
    Task ResetAsync();

    /// <summary>
    /// 禁止通行
    /// </summary>
    Task CloseAsync();

    /// <summary>
    /// 恢复通行
    /// </summary>
    Task OpenAsync();
}