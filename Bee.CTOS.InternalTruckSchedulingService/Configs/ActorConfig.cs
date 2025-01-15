using Phenix.Core;

namespace Bee.CTOS.InternalTruckSchedulingService.Configs;

/// <summary>
/// Actor配置
/// </summary>
public static class ActorConfig
{
    private static int? _actorIdleTimeout;

    /// <summary>
    /// 停用空闲 actor 前的超时（分钟）
    /// </summary>
    public static TimeSpan ActorIdleTimeout
    {
        get { return TimeSpan.FromMinutes(AppSettings.GetLocalProperty(ref _actorIdleTimeout, 60)); }
        set { AppSettings.SetLocalProperty(ref _actorIdleTimeout, value.TotalMinutes); }
    }

    private static int? _actorScanInterval;

    /// <summary>
    /// 持续时间，指定多久扫描一次 Actors，以停用闲置的 Actors（秒）
    /// </summary>
    public static TimeSpan ActorScanInterval
    {
        get { return TimeSpan.FromSeconds(AppSettings.GetLocalProperty(ref _actorScanInterval, 30)); }
        set { AppSettings.SetLocalProperty(ref _actorScanInterval, value.TotalSeconds); }
    }

    private static int? _drainOngoingCallTimeout;

    /// <summary>
    /// 重新平衡后的 Actors 重定位过程中的持续时间（秒）
    /// </summary>
    public static TimeSpan DrainOngoingCallTimeout
    {
        get { return TimeSpan.FromSeconds(AppSettings.GetLocalProperty(ref _drainOngoingCallTimeout, 60)); }
        set { AppSettings.SetLocalProperty(ref _drainOngoingCallTimeout, value.TotalSeconds); }
    }

    private static bool? _drainRebalancedActors;

    /// <summary>
    /// 如果为 true ，那么 Dapr 将等待 drainOngoingCallTimeout 以允许当前 actor 调用完成，然后再尝试停用 actor
    /// </summary>
    public static bool DrainRebalancedActors
    {
        get { return AppSettings.GetLocalProperty(ref _drainRebalancedActors, true); }
        set { AppSettings.SetLocalProperty(ref _drainRebalancedActors, value); }
    }

    private static int? _remindersStoragePartitions;

    /// <summary>
    /// 配置 actor 提醒的分区数量
    /// </summary>
    public static int RemindersStoragePartitions
    {
        get { return AppSettings.GetLocalProperty(ref _remindersStoragePartitions, 0); }
        set { AppSettings.SetLocalProperty(ref _remindersStoragePartitions, value); }
    }
}
