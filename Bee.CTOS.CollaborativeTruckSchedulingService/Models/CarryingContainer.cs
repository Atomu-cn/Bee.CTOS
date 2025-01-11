using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models
{
    /// <summary>
    /// 运输货柜
    /// </summary>
    [Sheet("CTS_CARRYING_CONTAINER")]
    public class CarryingContainer :  EntityBase<CarryingContainer>
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

        private readonly long _CCT_ID;

        /// <summary>
        /// 运输任务
        /// </summary>
        public long CCT_ID
        {
            get { return _CCT_ID; }
        }

        private readonly bool _isPlan;

        /// <summary>
        /// 是计划/实绩
        /// </summary>
        public bool IsPlan
        {
            get { return _isPlan; }
        }

        private readonly string _containerNumber;

        /// <summary>
        /// 箱号
        /// </summary>
        public string ContainerNumber
        {
            get { return _containerNumber; }
        }

        private readonly bool _isBigSize;

        /// <summary>
        /// 是大箱
        /// </summary>
        public bool IsBigSize
        {
            get { return _isBigSize; }
        }

        #endregion
    }
}