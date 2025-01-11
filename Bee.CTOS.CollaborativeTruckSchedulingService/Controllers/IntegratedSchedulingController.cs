using Dapr;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Bee.CTOS.CollaborativeTruckSchedulingService.Actors;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Controllers;

/// <summary>
/// 设备集成调度服务
/// </summary>
[ApiController]
public class IntegratedSchedulingController : ControllerBase
{
    private ITruckPoolsActor FetchTruckPoolsActor(string terminalNo, string truckPoolsNo)
    {
        ActorId actorId = new ActorId($"{{\"TerminalNo\":\"{terminalNo}\",\"TruckPoolsNo\":\"{truckPoolsNo}\"}}");
        return ActorProxy.Create<ITruckPoolsActor>(actorId, nameof(TruckPoolsActor));
    }

    #region Event

    /// <summary>
    /// 新的运输任务
    /// </summary>
    [Topic(Events.PubSubContracts.PubSubName, Events.PubSubContracts.NewCarryingTaskTopic)]
    [HttpPost(Events.PubSubContracts.NewCarryingTaskTopic)]
    public async Task<ActionResult> HandleNewCarryingTask(Events.CarryingTask msg)
    {
        await FetchTruckPoolsActor(msg.TerminalNo, msg.TruckPoolsNo).NewCarryingTaskAsync(msg);
        return Ok();
    }

    #endregion
}