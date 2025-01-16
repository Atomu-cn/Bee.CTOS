using Bee.CTOS.InternalTruckSchedulingService.Configs;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Newtonsoft.Json;

namespace Bee.CTOS.InternalTruckSchedulingService.Actors;

/// <summary>
/// 拓扑地图节点
/// ID: $"{{\"TerminalNo\":\"{terminalNo}\",\"Location\":\"{location}\"}}"
/// </summary>
public class TopologicalMapNodeActor : Actor, ITopologicalMapNodeActor
{
    public TopologicalMapNodeActor(ActorHost host)
        : base(host)
    {
        dynamic? topologicalMapNode = JsonConvert.DeserializeObject<dynamic>(this.Id.ToString());
        if (topologicalMapNode == null)
            throw new NotSupportedException($"本{this.GetType().FullName}不支持用'{this.Id}'格式构造TopologicalMapNodeActor对象!");

        _terminalNo = topologicalMapNode.TerminalNo;
        _location = topologicalMapNode.Location;
    }

    #region 属性

    private readonly string _terminalNo;
    private readonly string _location;

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
            _ownerLaneNos = await mapActor.FindOwnerLaneNosAsync(_location);
        if (_entryLaneNos == null)
            _entryLaneNos = await mapActor.FindEntryLaneNosAsync(_location);
        if ( _exitLaneNos == null)
            _exitLaneNos = await mapActor.FindExitLaneNosAsync(_location);

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