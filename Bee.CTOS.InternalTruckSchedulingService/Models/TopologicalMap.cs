using System.Data.Common;
using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.InternalTruckSchedulingService.Models;

/// <summary>
/// 拓扑地图
/// </summary>
[Sheet("ITS_TOPOLOGICAL_MAP")]
public class TopologicalMap : EntityBase<TopologicalMap>
{
    /// <summary>
    /// 创建
    /// </summary>
    public static TopologicalMap Create(string terminalNo)
    {
        TopologicalMap result = New(Set(p => p.TerminalNo, terminalNo));
        result.InsertSelf();
        return result;
    }

    #region 属性

    private readonly long _ID;

    /// <summary>
    /// ID
    /// </summary>
    public long ID
    {
        get { return _ID; }
    }

    private readonly string _terminalNo;

    /// <summary>
    /// 码头编号
    /// </summary>
    public string TerminalNo
    {
        get { return _terminalNo; }
    }

    #region Detail

    [NonSerialized]
    private IReadOnlyDictionary<string, TopologicalMapNode>? _nodeDict;

    /// <summary>
    /// 节点枚举: Location-TopologicalMapNode
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<string, TopologicalMapNode> NodeDict
    {
        get
        {
            if (_nodeDict == null)
            {
                IList<TopologicalMapNode> nodeList = this.FetchDetails<TopologicalMapNode>();
                Dictionary<string, TopologicalMapNode> result = new Dictionary<string, TopologicalMapNode>(nodeList.Count);
                foreach (TopologicalMapNode item in nodeList)
                    result[item.Location] = item;
                _nodeDict = result.AsReadOnly();
            }

            return _nodeDict;
        }
        private set { _nodeDict = value; }
    }

    [NonSerialized]
    private IReadOnlyDictionary<string, TopologicalMapLane>? _laneDict;

    /// <summary>
    /// 车道枚举: LaneNo-TopologicalMapLane
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<string, TopologicalMapLane> LaneDict
    {
        get
        {
            if (_laneDict == null)
            {
                IList<TopologicalMapLane> laneList = this.FetchDetails<TopologicalMapLane>();
                Dictionary<string, TopologicalMapLane> result = new Dictionary<string, TopologicalMapLane>(laneList.Count);
                foreach (TopologicalMapLane item in laneList)
                    result[item.LaneNo] = item;
                _laneDict = result.AsReadOnly();
            }

            return _laneDict;
        }
        private set { _laneDict = value; }
    }

    #endregion

    #endregion

    #region 方法

    private void ResetBy(TopologicalMapNode node)
    {
        if (_laneDict != null)
            foreach (KeyValuePair<string, TopologicalMapLane> kvp in _laneDict)
                kvp.Value.ResetBy(node);
    }

    private void ResetBy(TopologicalMapLane lane)
    {
        if (_nodeDict != null)
            foreach (KeyValuePair<string, TopologicalMapNode> kvp in _nodeDict)
                kvp.Value.ResetBy(lane);
    }

    /// <summary>
    /// Put节点
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    /// <param name="locationLng">经度（地图标记位置）</param>
    /// <param name="locationLat">纬度（地图标记位置）</param>
    /// <param name="nodeType">节点类型</param>
    public void PutNode(string location, double locationLng, double locationLat, TopologicalMapNodeType nodeType)
    {
        if (NodeDict.TryGetValue(location, out TopologicalMapNode? node))
        {
            node.UpdateSelf(
                TopologicalMapNode.Set(p => p.LocationLng, locationLng).
                    Set(p => p.LocationLat, locationLat).
                    Set(p => p.NodeType, nodeType));
            ResetBy(node);
        }
        else
        {
            node = this.NewDetail<TopologicalMapNode>(
                TopologicalMapNode.Set(p => p.Location, location).
                    Set(p => p.LocationLng, locationLng).
                    Set(p => p.LocationLat, locationLat).
                    Set(p => p.NodeType, nodeType));
            node.InsertSelf();
            NodeDict = new Dictionary<string, TopologicalMapNode>(NodeDict) { { location, node } }.AsReadOnly();
        }
    }

    /// <summary>
    /// Delete节点
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    public bool DeleteNode(string location)
    {
        if (NodeDict.TryGetValue(location, out TopologicalMapNode? node))
        {
            this.Database.Execute((DbTransaction transaction) => node.DeleteSelf(transaction));
            Dictionary<string, TopologicalMapNode> nodeDict = new Dictionary<string, TopologicalMapNode>(NodeDict);
            nodeDict.Remove(node.Location);
            NodeDict = nodeDict.AsReadOnly();
            ResetBy(node);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Put车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    /// <param name="count">经度（地图标记位置）</param>
    /// <param name="nodeLocations">节点位置集合（按LaneNo排列）</param>
    public void PutLane(string laneNo, int count, string[] nodeLocations)
    {
        if (LaneDict.TryGetValue(laneNo, out TopologicalMapLane? lane))
            lane.UpdateSelf(
                TopologicalMapLane.Set(p => p.Count, count));
        else
        {
            lane = this.NewDetail<TopologicalMapLane>(
                TopologicalMapLane.Set(p => p.LaneNo, laneNo).
                    Set(p => p.Count, count));
            lane.InsertSelf();
            LaneDict = new Dictionary<string, TopologicalMapLane>(LaneDict) { { laneNo, lane } }.AsReadOnly();
        }

        Dictionary<int, TopologicalMapNode> nodeDict = new Dictionary<int, TopologicalMapNode>(nodeLocations.Length);
        for (int i = 0; i < nodeLocations.Length; i++)
            if (NodeDict.TryGetValue(nodeLocations[i], out TopologicalMapNode? node))
                nodeDict.Add(i, node);
            else
                throw new InvalidOperationException($"位置{nodeLocations[i]}匹配不到现成的节点!");
        lane.NodeDict = nodeDict.AsReadOnly();
        ResetBy(lane);
    }

    /// <summary>
    /// Delete车道
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public bool DeleteLane(string laneNo)
    {
        if (LaneDict.TryGetValue(laneNo, out TopologicalMapLane? lane))
        {
            this.Database.Execute((DbTransaction transaction) => lane.DeleteSelf(transaction));
            Dictionary<string, TopologicalMapLane> laneDict = new Dictionary<string, TopologicalMapLane>(LaneDict);
            laneDict.Remove(lane.LaneNo);
            LaneDict = laneDict.AsReadOnly();
            ResetBy(lane);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 禁止通行
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public void CloseLane(string laneNo)
    {
        if (LaneDict.TryGetValue(laneNo, out TopologicalMapLane? lane))
            lane.Close();
    }

    /// <summary>
    /// 恢复通行
    /// </summary>
    /// <param name="laneNo">车道编号</param>
    public void OpenLane(string laneNo)
    {
        if (LaneDict.TryGetValue(laneNo, out TopologicalMapLane? lane))
            lane.Open();
    }

    #endregion
}