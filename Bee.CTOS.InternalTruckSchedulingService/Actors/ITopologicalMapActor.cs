using Dapr.Actors;
using Bee.CTOS.InternalTruckSchedulingService.Models;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图
/// </summary>
public interface ITopologicalMapActor : IActor
{
    /// <summary>
    /// 获取地图
    /// </summary>
    Task<TopologicalMap> FetchMapAsync();

    /// <summary>
    /// Put节点
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    /// <param name="locationLng">经度（地图标记位置）</param>
    /// <param name="locationLat">纬度（地图标记位置）</param>
    /// <param name="nodeType">节点类型</param>
    Task PutNodeAsync(string location, double locationLng, double locationLat, TopologicalMapNodeType nodeType);

    /// <summary>
    /// Delete节点
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    Task<bool> DeleteNodeAsync(string location);

    /// <summary>
    /// Put车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    /// <param name="count">经度（地图标记位置）</param>
    /// <param name="nodeLocations">节点位置集合（按LaneNo排列）</param>
    Task PutLaneAsync(string laneNo, int count, string[] nodeLocations);

    /// <summary>
    /// Delete车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    Task<bool> DeleteLaneAsync(string laneNo);

    /// <summary>
    /// 禁止通行
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    Task CloseLaneAsync(string laneNo);

    /// <summary>
    /// 恢复通行
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    Task OpenLaneAsync(string laneNo);
}