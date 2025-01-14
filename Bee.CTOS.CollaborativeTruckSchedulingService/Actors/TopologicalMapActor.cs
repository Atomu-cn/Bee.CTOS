using Dapr.Actors.Runtime;
using Bee.CTOS.CollaborativeTruckSchedulingService.Models;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图
/// ID: TerminalNo
/// </summary>
public class TopologicalMapActor : Actor, ITopologicalMapActor
{
    public TopologicalMapActor(ActorHost host)
        : base(host)
    {
        string terminalNo = this.Id.ToString();
        _topologicalMap = TopologicalMap.FetchRoot(p => p.TerminalNo == terminalNo) ?? TopologicalMap.Create(terminalNo);
    }

    #region 属性

    private readonly TopologicalMap _topologicalMap;

    #endregion

    #region 方法

    #region API

    /// <summary>
    /// 创建或覆盖地图
    /// </summary>
    public Task<TopologicalMap> FetchMapAsync()
    {
        return Task.FromResult(_topologicalMap);
    }

    /// <summary>
    /// Put节点
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    /// <param name="locationLng">经度（地图标记位置）</param>
    /// <param name="locationLat">纬度（地图标记位置）</param>
    /// <param name="nodeType">节点类型</param>
    public Task PutNodeAsync(string location, double locationLng, double locationLat, TopologicalMapNodeType nodeType)
    {
        _topologicalMap.PutNode(location, locationLng, locationLat, nodeType);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete节点
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    public Task<bool> DeleteNodeAsync(string location)
    {
        return Task.FromResult(_topologicalMap.DeleteNode(location));
    }

    /// <summary>
    /// Put车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    /// <param name="count">经度（地图标记位置）</param>
    /// <param name="nodeLocations">节点位置集合（按LaneNo排列）</param>
    public Task PutLaneAsync(string laneNo, int count, string[] nodeLocations)
    {
        _topologicalMap.PutLane(laneNo, count, nodeLocations);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public Task<bool> DeleteLaneAsync(string laneNo)
    {
        return Task.FromResult(_topologicalMap.DeleteLane(laneNo));
    }

    public Task CloseLaneAsync(string laneNo)
    {
        _topologicalMap.CloseLane(laneNo);
        return Task.CompletedTask;
    }

    public Task OpenLaneAsync(string laneNo)
    {
        _topologicalMap.OpenLane(laneNo);
        return Task.CompletedTask;
    }

    #endregion

    #endregion
}