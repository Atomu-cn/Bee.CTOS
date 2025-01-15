using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Bee.CTOS.InternalTruckSchedulingService.Actors;
using Bee.CTOS.InternalTruckSchedulingService.Models;

namespace Bee.CTOS.InternalTruckSchedulingService.Controllers
{
    /// <summary>
    /// 集卡池服务
    /// </summary>
    [ApiController]
    [Route("/api/its/truck-pools")]
    public sealed class TruckPoolsController : Phenix.Core.Net.Api.ControllerBase
    {
        private ITruckPoolsActor FetchTruckPoolsActor(string terminalNo, string truckPoolsNo)
        {
            ActorId actorId = new ActorId($"{{\"TerminalNo\":\"{terminalNo}\",\"TruckPoolsNo\":\"{truckPoolsNo}\"}}");
            return ActorProxy.Create<ITruckPoolsActor>(actorId, nameof(TruckPoolsActor));
        }

        #region API

        /// <summary>
        /// 获取全部（含作废）
        /// </summary>
        /// <returns>集卡池清单</returns>
        [HttpGet("all")]
        public ActionResult<TruckPools[]> GetAll()
        {
            return TruckPools.FetchList().ToArray();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="truckPoolsNo">集卡池号</param>
        /// <returns>集卡池</returns>
        [HttpGet]
        public ActionResult<TruckPools> Get(string terminalNo, string truckPoolsNo)
        {
            TruckPools? result = TruckPools.FetchRoot(p => p.TerminalNo == terminalNo && p.TruckPoolsNo == truckPoolsNo);
            return result != null ? result : NotFound();
        }

        /// <summary>
        /// Put
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="truckPoolsNo">集卡池号</param>
        /// <param name="truckNos">集卡编号清单</param>
        [HttpPut]
        public async Task<ActionResult> Put(string terminalNo, string truckPoolsNo, string[] truckNos)
        {
            await FetchTruckPoolsActor(terminalNo, truckPoolsNo).PutAsync(truckNos);
            return Ok();
        }

        /// <summary>
        /// 作废
        /// </summary>
        [HttpPost("invalid")]
        public async Task<ActionResult> Invalid(string terminalNo, string truckPoolsNo)
        {
            await FetchTruckPoolsActor(terminalNo, truckPoolsNo).InvalidAsync();
            return Ok();
        }

        /// <summary>
        /// 恢复
        /// </summary>
        [HttpPost("resume")]
        public async Task<ActionResult> Resume(string terminalNo, string truckPoolsNo)
        {
            await FetchTruckPoolsActor(terminalNo, truckPoolsNo).ResumeAsync();
            return Ok();
        }

        #endregion
    }
}