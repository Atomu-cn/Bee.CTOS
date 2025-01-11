using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models;

/// <summary>
/// 拓扑地图车道节点
/// </summary>
[Sheet("CTS_TOPOLOGICAL_MAP_LANE_NODE")]
public class TopologicalMapLaneNode : EntityBase<TopologicalMapLaneNode>
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

    private readonly long _CTL_ID;

    /// <summary>
    /// 车道
    /// </summary>
    public long CTL_ID
    {
        get { return _CTL_ID; }
    }

    private readonly int _orderNo;

    /// <summary>
    /// 顺序号（从入口到出口）
    /// </summary>
    public int OrderNo
    {
        get { return _orderNo; }
    }

    private readonly long _CTN_ID;

    /// <summary>
    /// 节点
    /// </summary>
    public long CTN_ID
    {
        get { return _CTN_ID; }
    }

    #endregion
}