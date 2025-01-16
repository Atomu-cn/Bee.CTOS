using Phenix.Core;

namespace Bee.CTOS.InternalTruckSchedulingService.Configs;

/// <summary>
/// 自动机配置
/// </summary>
public static class AutoRunConfig
{
    private static int? _truckPoolsActorAliveInterval;

    /// <summary>
    /// 集卡池活动间隔（秒）
    /// </summary>
    public static TimeSpan TruckPoolsActorAliveInterval
    {
        get { return TimeSpan.FromSeconds(AppSettings.GetLocalProperty(ref _truckPoolsActorAliveInterval, 3)); }
        set { AppSettings.SetLocalProperty(ref _truckPoolsActorAliveInterval, value.TotalSeconds); }
    }

    private static int? _topologicalMapNodeActorAliveInterval;

    /// <summary>
    /// 拓扑地图节点活动间隔（秒）
    /// </summary>
    public static TimeSpan TopologicalMapNodeActorAliveInterval
    {
        get { return TimeSpan.FromSeconds(AppSettings.GetLocalProperty(ref _topologicalMapNodeActorAliveInterval, 3)); }
        set { AppSettings.SetLocalProperty(ref _topologicalMapNodeActorAliveInterval, value.TotalSeconds); }
    }
}
