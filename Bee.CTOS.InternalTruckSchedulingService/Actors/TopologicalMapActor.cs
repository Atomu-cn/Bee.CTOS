using Bee.CTOS.InternalTruckSchedulingService.Models;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

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

    private async Task ResetGraphAsync()
    {
        List<Task> tasks = new List<Task>(_topologicalMap.NodeDict.Count);
        foreach (KeyValuePair<string, TopologicalMapNode> kvp in _topologicalMap.NodeDict)
        {
            ActorId actorId = new ActorId($"{{\"TerminalNo\":\"{_topologicalMap.TerminalNo}\",\"Location\":\"{kvp.Value.Location}\"}}");
            tasks.Add(Task.Run(() => this.ProxyFactory.CreateActorProxy<ITopologicalMapNodeActor>(actorId, nameof(TopologicalMapNodeActor)).ResetAsync()));
        }

        await Task.WhenAll(tasks.ToArray());
    }

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
    public async Task DeleteNodeAsync(string location)
    {
        bool result = _topologicalMap.DeleteNode(location);
        if (result)
            await ResetGraphAsync();
    }

    /// <summary>
    /// Put车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    /// <param name="count">经度（地图标记位置）</param>
    /// <param name="nodeLocations">节点位置集合（按LaneNo排列）</param>
    public async Task PutLaneAsync(string laneNo, int count, string[] nodeLocations)
    {
        _topologicalMap.PutLane(laneNo, count, nodeLocations);
        await ResetGraphAsync();
    }

    /// <summary>
    /// Delete车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public async Task DeleteLaneAsync(string laneNo)
    {
        bool result = _topologicalMap.DeleteLane(laneNo);
        if (result)
            await ResetGraphAsync();
    }

    /// <summary>
    /// 禁止通行
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public async Task CloseLaneAsync(string laneNo)
    {
        bool result = _topologicalMap.CloseLane(laneNo);
        if (result)
            await ResetGraphAsync();
    }

    /// <summary>
    /// 恢复通行
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public async Task OpenLaneAsync(string laneNo)
    {
        bool result = _topologicalMap.OpenLane(laneNo);
        if (result)
            await ResetGraphAsync();
    }

    /// <summary>
    /// 查找节点所属车道集合
    /// </summary>
    /// <param name="nodeLocation">节点位置</param>
    public Task<string[]?> FindOwnerLaneNosAsync(string nodeLocation)
    {
        return Task.FromResult(_topologicalMap.NodeDict.TryGetValue(nodeLocation, out TopologicalMapNode? node)
            ? node.OwnerLaneDict.Keys.ToArray()
            : null);
    }

    /// <summary>
    /// 查找节点入口车道集合
    /// </summary>
    /// <param name="nodeLocation">节点位置</param>
    public Task<string[]?> FindEntryLaneNosAsync(string nodeLocation)
    {
        return Task.FromResult(_topologicalMap.NodeDict.TryGetValue(nodeLocation, out TopologicalMapNode? node)
            ? node.EntryLaneDict.Keys.ToArray()
            : null);
    }

    /// <summary>
    /// 查找节点出口车道集合
    /// </summary>
    /// <param name="nodeLocation">节点位置</param>
    public Task<string[]?> FindExitLaneNosAsync(string nodeLocation)
    {
        return Task.FromResult(_topologicalMap.NodeDict.TryGetValue(nodeLocation, out TopologicalMapNode? node)
            ? node.ExitLaneDict.Keys.ToArray()
            : null);
    }

    #endregion

    #endregion
}