using Bee.CTOS.InternalTruckSchedulingService.Configs;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Newtonsoft.Json;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图车道（单向）
/// ID: $"{{\"TerminalNo\":\"{terminalNo}\",\"LaneNo\":\"{laneNo}\"}}"
/// </summary>
public class TopologicalMapLaneActor : Actor, ITopologicalMapNodeActor
{
    public TopologicalMapLaneActor(ActorHost host)
        : base(host)
    {
        dynamic? topologicalMapLane = JsonConvert.DeserializeObject<dynamic>(this.Id.ToString());
        if (topologicalMapLane == null)
            throw new NotSupportedException($"本{this.GetType().FullName}不支持用'{this.Id}'格式构造TopologicalMapLaneActor对象!");

        _terminalNo = topologicalMapLane.TerminalNo;
        _laneNo = topologicalMapLane.LaneNo;
    }

    #region 属性

    private readonly string _terminalNo;
    private readonly string _laneNo;

    private string[]? _ownerLaneNos;
    private string[]? _entryLaneNos;
    private string[]? _exitLaneNos;
    
    private ActorTimer? _timer;

    #endregion

    #region 方法

    private ITopologicalMapActor FetchTopologicalMapActor()
    {
        ActorId actorId = new ActorId(_terminalNo);
        return this.ProxyFactory.CreateActorProxy<ITopologicalMapActor>(actorId, nameof(TopologicalMapActor));
    }

    #region AutoRun

    private async Task RegisterTimerAsync()
    {
        if (_timer == null)
            _timer = await this.RegisterTimerAsync(this.Id.ToString(), nameof(OnTimerCallBack), null, AutoRunConfig.TopologicalMapNodeActorAliveInterval, AutoRunConfig.TopologicalMapNodeActorAliveInterval);
    }

    private async Task UnRegisterTimerAsync()
    {
        if (_timer != null)
        {
            await this.UnregisterTimerAsync(_timer);
            _timer = null;
        }
    }

    private async Task OnTimerCallBack(byte[] data)
    {
        ITopologicalMapActor mapActor = FetchTopologicalMapActor();
        if (_ownerLaneNos == null)
            _ownerLaneNos = await mapActor.FindOwnerLaneNosAsync(_laneNo);
        if (_entryLaneNos == null)
            _entryLaneNos = await mapActor.FindEntryLaneNosAsync(_laneNo);
        if ( _exitLaneNos == null)
            _exitLaneNos = await mapActor.FindExitLaneNosAsync(_laneNo);

    }
    
    #endregion

    #region API

    /// <summary>
    /// 重置
    /// </summary>
    public async Task ResetAsync()
    {
        await UnRegisterTimerAsync();
        _ownerLaneNos = null;
        _entryLaneNos = null;
        _exitLaneNos = null;
        await RegisterTimerAsync();
    }

    #endregion

    #endregion
}