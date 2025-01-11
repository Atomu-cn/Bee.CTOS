using System.Data.Common;
using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Expressions;
using Phenix.Core.Mapper.Schema;
using Bee.CTOS.CollaborativeTruckSchedulingService.Common;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models;

/// <summary>
/// 拓扑地图车道（单向）
/// </summary>
[Sheet("CTS_TOPOLOGICAL_MAP_LANE")]
public class TopologicalMapLane : EntityBase<TopologicalMapLane>
{
    #region 属性

    private readonly long _ID;

    /// <summary>
    /// ID
    /// </summary>
    public long ID
    {
        get { return _ID; }
    }

    private readonly long _CTM_ID;

    /// <summary>
    /// 拓扑地图
    /// </summary>
    public long CTM_ID
    {
        get { return _CTM_ID; }
    }

    private readonly string _laneNo;

    /// <summary>
    /// 车道编号
    /// </summary>
    public string LaneNo
    {
        get { return _laneNo; }
    }

    private readonly int _count;

    /// <summary>
    /// 车道数量
    /// </summary>
    public int Count
    {
        get { return _count; }
    }

    #region Detail

    [NonSerialized]
    private IReadOnlyDictionary<int, TopologicalMapNode>? _nodeDict;

    /// <summary>
    /// 节点枚举: OrderNo-TopologicalMapNode
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<int, TopologicalMapNode> NodeDict
    {
        get
        {
            if (_nodeDict == null)
            {
                SortedDictionary<int, TopologicalMapNode> result = new SortedDictionary<int, TopologicalMapNode>();
                IList<TopologicalMapLaneNode> laneNodeList = this.FetchDetails<TopologicalMapLaneNode>(
                    OrderBy.Ascending<TopologicalMapLaneNode>(p => p.OrderNo));
                foreach (KeyValuePair<string, TopologicalMapNode> kvp in ((TopologicalMap)Master).NodeDict)
                {
                    TopologicalMapLaneNode? laneNode = null;
                    foreach (TopologicalMapLaneNode item in laneNodeList)
                        if (kvp.Value.ID == item.CTN_ID)
                        {
                            laneNode = item;
                            result.Add(item.OrderNo, kvp.Value);
                            break;
                        }

                    if (laneNode != null)
                    {
                        laneNodeList.Remove(laneNode);
                        if (laneNodeList.Count == 0)
                            break;
                    }
                }

                _nodeDict = result.AsReadOnly();
            }

            return _nodeDict;
        }
        set
        {
            this.Database.Execute((DbTransaction transaction) =>
            {
                this.DeleteDetails<TopologicalMapLaneNode>(transaction);
                foreach (KeyValuePair<int, TopologicalMapNode> kvp in value)
                {
                    TopologicalMapLaneNode laneNode = this.NewDetail<TopologicalMapLaneNode>(
                        TopologicalMapLaneNode.Set(p => p.OrderNo, kvp.Key).
                            Set(p => p.CTN_ID, kvp.Value.ID));
                    laneNode.InsertSelf(transaction);
                }
            });
            _nodeDict = value;
            _nodeDistanceDict = null;
        }
    }

    /// <summary>
    /// 入口节点
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public TopologicalMapNode? EntryNode
    {
        get { return NodeDict.Count > 0 ? NodeDict.First().Value : null; }
    }

    /// <summary>
    /// 出口节点
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public TopologicalMapNode? ExitNode
    {
        get { return NodeDict.Count > 0 ? NodeDict.Last().Value : null; }
    }

    [NonSerialized]
    private IReadOnlyDictionary<TopologicalMapNode, double>? _nodeDistanceDict;

    /// <summary>
    ///  从入口到节点的行驶距离
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<TopologicalMapNode, double> NodeDistanceDict
    {
        get
        {
            if (_nodeDistanceDict == null)
            {
                Dictionary<TopologicalMapNode, double> result = new Dictionary<TopologicalMapNode, double>(NodeDict.Count);
                TopologicalMapNode? priorNode = EntryNode;
                if (priorNode != null)
                {
                    double distance = 0;
                    foreach (KeyValuePair<int, TopologicalMapNode> kvp in NodeDict)
                    {
                        distance = distance + MapHelper.GetDistance(priorNode.LocationLat, priorNode.LocationLng, kvp.Value.LocationLat, kvp.Value.LocationLng);
                        result.Add(kvp.Value, distance);
                        priorNode = kvp.Value;
                    }
                }

                _nodeDistanceDict = result.AsReadOnly();
            }

            return _nodeDistanceDict;
        }
    }

    #endregion

    #endregion

    #region 方法

    internal void ResetRelate(TopologicalMapNode node)
    {
        if (_nodeDistanceDict != null && _nodeDistanceDict.ContainsKey(node))
        {
            _nodeDict = null;
            _nodeDistanceDict = null;
        }
    }

    internal void DeleteSelf(DbTransaction transaction)
    {
        this.DeleteDetails<TopologicalMapLaneNode>(transaction);
        base.DeleteSelf(transaction);
    }

    #endregion
}