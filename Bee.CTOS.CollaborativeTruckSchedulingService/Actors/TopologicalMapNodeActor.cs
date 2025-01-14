using Dapr.Actors.Runtime;
using Newtonsoft.Json;
using Bee.CTOS.CollaborativeTruckSchedulingService.Models;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图节点
/// ID: $"{{\"TerminalNo\":\"{terminalNo}\",\"Location\":\"{location}\"}}"
/// </summary>
public class TopologicalMapNodeActor : Actor, ITopologicalMapActor
{
    public TopologicalMapNodeActor(ActorHost host)
        : base(host)
    {
        dynamic? topologicalMapJunction = JsonConvert.DeserializeObject<dynamic>(this.Id.ToString());
        if (topologicalMapJunction == null)
            throw new NotSupportedException($"本{this.GetType().FullName}不支持用'{this.Id}'格式构造TruckPools对象!");

        _terminalNo = this.Id.ToString();
        _location = this.Id.ToString();
        _topologicalMap = TopologicalMap.FetchRoot(p => p.TerminalNo == _terminalNo) ?? TopologicalMap.Create(_terminalNo);
    }

    #region 属性

    private readonly string _terminalNo;
    private readonly string _location;
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

    #endregion

    #endregion
}