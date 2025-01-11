using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Bee.CTOS.CollaborativeTruckSchedulingService.Actors;
using Bee.CTOS.CollaborativeTruckSchedulingService.Models;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Controllers
{
    /// <summary>
    /// 拓扑地图服务
    /// </summary>
    [ApiController]
    [Route("/api/cts/topological-map")]
    public sealed class TopologicalMapController : Phenix.Core.Net.Api.ControllerBase
    {
        private ITopologicalMapActor FetchTopologicalMapActor(string terminalNo)
        {
            ActorId actorId = new ActorId(terminalNo);
            return ActorProxy.Create<ITopologicalMapActor>(actorId, nameof(TopologicalMapActor));
        }

        #region API

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <returns>拓扑地图</returns>
        [HttpGet]
        public ActionResult<TopologicalMap> Get(string terminalNo)
        {
            TopologicalMap? result = TopologicalMap.FetchRoot(p => p.TerminalNo == terminalNo);
            return result != null ? result : NotFound();
        }

        /// <summary>
        /// Put节点
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="location">位置（地图标记位置）</param>
        /// <param name="locationLng">经度（地图标记位置）</param>
        /// <param name="locationLat">纬度（地图标记位置）</param>
        /// <param name="nodeType">节点类型</param>
        [HttpPut("node")]
        public async Task<ActionResult> PutNode(string terminalNo, string location, double locationLng, double locationLat, TopologicalMapNodeType nodeType)
        {
            await FetchTopologicalMapActor(terminalNo).PutNodeAsync(location, locationLng, locationLat, nodeType);
            return Ok();
        }

        /// <summary>
        /// Delete节点
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="location">位置（地图标记位置）</param>
        [HttpDelete("node")]
        public async Task<ActionResult<bool>> DeleteNode(string terminalNo, string location)
        {
            return await FetchTopologicalMapActor(terminalNo).DeleteNodeAsync(location);
        }

        /// <summary>
        /// Put车道
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="laneNo">车道编号</param>
        /// <param name="count">经度（地图标记位置）</param>
        /// <param name="nodeLocations">节点位置集合（按LaneNo排列）</param>
        [HttpPut("lane")]
        public async Task<ActionResult> PutLane(string terminalNo, string laneNo, int count, string[] nodeLocations)
        {
            await FetchTopologicalMapActor(terminalNo).PutLaneAsync(laneNo, count, nodeLocations);
            return Ok();
        }

        /// <summary>
        /// Delete车道
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="laneNo">车道编号</param>
        [HttpDelete("lane")]
        public async Task<ActionResult<bool>> DeleteLane(string terminalNo, string laneNo)
        {
            return await FetchTopologicalMapActor(terminalNo).DeleteLaneAsync(laneNo);
        }

        #endregion
    }
}