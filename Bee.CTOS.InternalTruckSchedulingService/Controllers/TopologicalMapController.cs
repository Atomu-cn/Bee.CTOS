using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Bee.CTOS.InternalTruckSchedulingService.Actors;
using Bee.CTOS.InternalTruckSchedulingService.Models;

namespace Bee.CTOS.InternalTruckSchedulingService.Controllers
{
    /// <summary>
    /// 拓扑地图服务
    /// </summary>
    [ApiController]
    [Route("/api/its/topological-map")]
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
        public async Task<ActionResult<TopologicalMap>> Get(string terminalNo)
        {
            return await FetchTopologicalMapActor(terminalNo).FetchMapAsync();
        }

        /// <summary>
        /// 获取车道集合
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <returns>车道集合</returns>
        [HttpGet("lanes")]
        public async Task<ActionResult<TopologicalMapLane[]>> GetLanes(string terminalNo)
        {
            return await FetchTopologicalMapActor(terminalNo).FetchLanesAsync();
        }

        /// <summary>
        /// 获取车道节点集合
        /// </summary>
        /// <param name="terminalNo">码头编号</param>
        /// <param name="laneNo">车道编号</param>
        /// <returns>节点集合</returns>
        [HttpGet("lane-nodes")]
        public async Task<ActionResult<TopologicalMapNode[]?>> GetLaneNodes(string terminalNo, string laneNo)
        {
            return await FetchTopologicalMapActor(terminalNo).FetchLaneNodesAsync(laneNo);
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
        public async Task<ActionResult> DeleteNode(string terminalNo, string location)
        {
            await FetchTopologicalMapActor(terminalNo).DeleteNodeAsync(location);
            return Ok();
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
        public async Task<ActionResult> DeleteLane(string terminalNo, string laneNo)
        {
            await FetchTopologicalMapActor(terminalNo).DeleteLaneAsync(laneNo);
            return Ok();
        }

        /// <summary>
        /// 禁止通行
        /// </summary>
        [HttpPost("close-lane")]
        public async Task<ActionResult> CloseLane(string terminalNo, string laneNo)
        {
            await FetchTopologicalMapActor(terminalNo).CloseLaneAsync(laneNo);
            return Ok();
        }

        /// <summary>
        /// 恢复通行
        /// </summary>
        [HttpPost("open-lane")]
        public async Task<ActionResult> OpenLane(string terminalNo, string laneNo)
        {
            await FetchTopologicalMapActor(terminalNo).OpenLaneAsync(laneNo);
            return Ok();
        }

        #endregion
    }
}