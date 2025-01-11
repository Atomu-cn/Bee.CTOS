using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models
{
    /// <summary>
    /// 运输任务作业
    /// </summary>
    [Sheet("CTS_CARRYING_TASK_OPERATION")]
    public class CarryingTaskOperation : EntityBase<CarryingTaskOperation>
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

        private readonly long _CCO_ID;

        /// <summary>
        /// 运输任务指令
        /// </summary>
        public long CCO_ID
        {
            get { return _CCO_ID; }
        }

        private readonly CarryingTaskOperationStatus _status;

        /// <summary>
        /// 作业状态
        /// </summary>
        public CarryingTaskOperationStatus Status
        {
            get { return _status; }
        }

        private readonly DateTime _timestamp;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        #endregion
    }
}