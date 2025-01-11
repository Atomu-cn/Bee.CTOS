using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Bee.CTOS.CollaborativeTruckSchedulingService.Actors;
using Bee.CTOS.CollaborativeTruckSchedulingService.Models;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Controllers
{
    /// <summary>
    /// 集卡服务
    /// </summary>
    [ApiController]
    [Route("/api/cts/truck")]
    public sealed class TruckController : Phenix.Core.Net.Api.ControllerBase
    {
        private ITruckActor FetchTruckActor(string truckNo)
        {
            ActorId actorId = new ActorId(truckNo);
            return ActorProxy.Create<ITruckActor>(actorId, nameof(TruckActor));
        }

        #region API

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <returns>集卡清单</returns>
        [HttpGet("all")]
        public ActionResult<Truck[]> GetAll()
        {
            return Truck.FetchList().ToArray();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="truckNo">集卡编号</param>
        /// <returns>集卡</returns>
        [HttpGet]
        public ActionResult<Truck> Get(string truckNo)
        {
            Truck? result = Truck.FetchRoot(p => p.TruckNo == truckNo);
            return result != null ? result : NotFound();
        }

        /// <summary>
        /// Put
        /// </summary>
        /// <param name="truckNo">集卡编号</param>
        /// <param name="driveType">驾驶类型</param>
        [HttpPut]
        public async Task<ActionResult> Put(string truckNo, TruckDriveType driveType)
        {
            await FetchTruckActor(truckNo).PutAsync(driveType);
            return Ok();
        }

        /// <summary>
        /// 更改健康状态
        /// </summary>
        [HttpPatch]
        public async Task<ActionResult> ChangeHealthStatus(string truckNo, TruckHealthStatus healthStatus)
        {
            await FetchTruckActor(truckNo).ChangeHealthStatusAsync(healthStatus);
            return Ok();
        }
        
        #endregion
    }
}