using System.Data.Common;
using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models;

/// <summary>
/// 拓扑地图节点
/// </summary>
[Sheet("CTS_TOPOLOGICAL_MAP_NODE")]
public class TopologicalMapNode : EntityBase<TopologicalMapNode>
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

    private readonly string _location;

    /// <summary>
    /// 位置（地图标记位置）
    /// </summary>
    public string Location
    {
        get { return _location; }
    }

    private readonly double _locationLng;

    /// <summary>
    /// 经度（地图标记位置）
    /// </summary>
    public double LocationLng
    {
        get { return _locationLng; }
    }

    private readonly double _locationLat;

    /// <summary>
    /// 纬度（地图标记位置）
    /// </summary>
    public double LocationLat
    {
        get { return _locationLat; }
    }

    private readonly TopologicalMapNodeType _nodeType;

    /// <summary>
    /// 节点类型
    /// </summary>
    public TopologicalMapNodeType NodeType
    {
        get { return _nodeType; }
    }

    #region Detail

    [NonSerialized]
    private IReadOnlyDictionary<string, TopologicalMapLane>? _ownerLaneDict;

    /// <summary>
    /// 所属车道枚举: LaneNo-TopologicalMapLane
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<string, TopologicalMapLane> OwnerLaneDict
    {
        get
        {
            if (_ownerLaneDict == null)
            {
                SortedDictionary<string, TopologicalMapLane> result = new SortedDictionary<string, TopologicalMapLane>();
                IList<TopologicalMapLaneNode> laneNodeList = this.FetchDetails<TopologicalMapLaneNode>();
                foreach (KeyValuePair<string, TopologicalMapLane> kvp in ((TopologicalMap)Master).LaneDict)
                {
                    TopologicalMapLaneNode? laneNode = null;
                    foreach (TopologicalMapLaneNode item in laneNodeList)
                        if (kvp.Value.ID == item.CTL_ID)
                        {
                            laneNode = item;
                            result.Add(kvp.Key, kvp.Value);
                            break;
                        }

                    if (laneNode != null)
                    {
                        laneNodeList.Remove(laneNode);
                        if (laneNodeList.Count == 0)
                            break;
                    }
                }

                _ownerLaneDict = result.AsReadOnly();
            }

            return _ownerLaneDict;
        }
    }

    [NonSerialized]
    private IReadOnlyDictionary<string, TopologicalMapLane>? _entryLaneDict;

    /// <summary>
    /// 入口车道枚举: LaneNo-TopologicalMapLane
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<string, TopologicalMapLane> EntryLaneDict
    {
        get
        {
            if (_entryLaneDict == null)
            {
                SortedDictionary<string, TopologicalMapLane> result = new SortedDictionary<string, TopologicalMapLane>();
                foreach (KeyValuePair<string, TopologicalMapLane> kvp in OwnerLaneDict)
                    if (kvp.Value.ExitNode != null && kvp.Value.ExitNode.ID == ID)
                        result.Add(kvp.Key, kvp.Value);

                _entryLaneDict = result.AsReadOnly();
            }

            return _entryLaneDict;
        }
    }

    [NonSerialized]
    private IReadOnlyDictionary<string, TopologicalMapLane>? _exitLaneDict;

    /// <summary>
    /// 出口车道枚举: LaneNo-TopologicalMapLane
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public IReadOnlyDictionary<string, TopologicalMapLane> ExitLaneDict
    {
        get
        {
            if (_exitLaneDict == null)
            {
                SortedDictionary<string, TopologicalMapLane> result = new SortedDictionary<string, TopologicalMapLane>();
                foreach (KeyValuePair<string, TopologicalMapLane> kvp in OwnerLaneDict)
                    if (kvp.Value.EntryNode != null && kvp.Value.EntryNode.ID == ID)
                        result.Add(kvp.Key, kvp.Value);

                _exitLaneDict = result.AsReadOnly();
            }

            return _exitLaneDict;
        }
    }

    #endregion

    #endregion

    #region 方法

    internal void ResetRelate(TopologicalMapLane lane)
    {
        if (_ownerLaneDict != null && _ownerLaneDict.ContainsKey(lane.LaneNo))
        {
            _ownerLaneDict = null;
            _entryLaneDict = null;
            _exitLaneDict = null;
        }
    }

    internal void DeleteSelf(DbTransaction transaction)
    {
        this.DeleteDetails<TopologicalMapLaneNode>(transaction);
        base.DeleteSelf(transaction);
    }

    #endregion
}