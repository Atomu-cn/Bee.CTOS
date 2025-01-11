using System.Data.Common;
using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.CollaborativeTruckSchedulingService.Models;

/// <summary>
/// 运输任务
/// </summary>
[Sheet("CTS_CARRYING_TASK")]
public class CarryingTask : EntityBase<CarryingTask>
{
    /// <summary>
    /// 创建
    /// </summary>
    public static CarryingTask Create(string terminalNo, string truckPoolsNo,
        long taskId, CarryingTaskType taskType, int taskPriority, TruckLoadingPosition? planLoadingPosition,
        string containerNumber, bool isBigSize,
        string loadLocation, long loadQueueNo, DateTime loadPlanTime, string? loadCraneNo,
        string unloadLocation, long unloadQueueNo, DateTime unloadPlanTime, string? unloadCraneNo, bool needTwistLock,
        QuayCraneProcess? quayCraneProcess)
    {
        if (taskType == CarryingTaskType.Shift && quayCraneProcess != null)
            throw new InvalidOperationException($"转堆作业时不应该有岸桥工艺!");
        if (taskType != CarryingTaskType.Shift && quayCraneProcess == null)
            throw new InvalidOperationException($"非转堆作业时应该有岸桥工艺!");

        CarryingTask result = New(Set(p => p.TerminalNo, terminalNo).
            Set(p => p.TruckPoolsNo, truckPoolsNo).
            Set(p => p.TaskId, taskId).
            Set(p => p.TaskType, taskType).
            Set(p => p.TaskPriority, taskPriority).
            Set(p => p.PlanLoadingPosition, planLoadingPosition).
            Set(p => p.OriginateTime, DateTime.Now));

        result._containerDict[true] = result.NewDetail<CarryingContainer>(
            CarryingContainer.Set(p => p.IsPlan, true).
                Set(p => p.ContainerNumber, containerNumber).
                Set(p => p.IsBigSize, isBigSize));
        result._containerDict[false] = null;

        result._orderDict[CarryingTaskOrderType.Load] = result.NewDetail<CarryingTaskOrder>(
            CarryingTaskOrder.Set(p => p.OrderType, CarryingTaskOrderType.Load).
                Set(p => p.LoadUnloadLocation, loadLocation).
                Set(p => p.LoadUnloadQueueNo, loadQueueNo).
                Set(p => p.LoadUnloadPlanTime, loadPlanTime).
                Set(p => p.CraneNo, loadCraneNo).
                Set(p => p.CraneType, taskType is CarryingTaskType.Discharge or CarryingTaskType.Transship ? CraneType.QuayCrane : CraneType.GantryCrane).
                Set(p => p.QuayCraneProcess, taskType is CarryingTaskType.Discharge or CarryingTaskType.Transship ? quayCraneProcess : null).
                Set(p => p.NeedTwistLock, false));
        result._orderDict[CarryingTaskOrderType.Unload] = result.NewDetail<CarryingTaskOrder>(
            CarryingTaskOrder.Set(p => p.OrderType, CarryingTaskOrderType.Unload).
                Set(p => p.LoadUnloadLocation, unloadLocation).
                Set(p => p.LoadUnloadQueueNo, unloadQueueNo).
                Set(p => p.LoadUnloadPlanTime, unloadPlanTime).
                Set(p => p.CraneNo, unloadCraneNo).
                Set(p => p.CraneType, taskType is CarryingTaskType.Shipment or CarryingTaskType.Transship ? CraneType.QuayCrane : CraneType.GantryCrane).
                Set(p => p.QuayCraneProcess, taskType is CarryingTaskType.Shipment or CarryingTaskType.Transship ? quayCraneProcess : null).
                Set(p => p.NeedTwistLock, needTwistLock && taskType != CarryingTaskType.Shift));

        result.Database.Execute((DbTransaction transaction) =>
        {
            result.InsertSelf(transaction);
            foreach (KeyValuePair<bool, CarryingContainer?> kvp in result._containerDict)
                if (kvp.Value != null)
                    kvp.Value.InsertSelf(transaction);
            foreach (KeyValuePair<CarryingTaskOrderType, CarryingTaskOrder> kvp in result._orderDict)
                kvp.Value.InsertSelf(transaction);
        });

        return result;
    }

    #region 属性

    private readonly long _ID;

    /// <summary>
    /// ID
    /// </summary>
    public long ID
    {
        get { return _ID; }
    }

    private readonly string _terminalNo;

    /// <summary>
    /// 码头编号
    /// </summary>
    public string TerminalNo
    {
        get { return _terminalNo; }
    }

    private readonly string _truckPoolsNo;

    /// <summary>
    /// 集卡池号
    /// </summary>
    public string TruckPoolsNo
    {
        get { return _truckPoolsNo; }
    }

    private readonly long _taskId;

    /// <summary>
    /// 任务ID
    /// </summary>
    public long TaskId
    {
        get { return _taskId; }
    }

    private readonly CarryingTaskType _taskType;

    /// <summary>
    /// 任务类型
    /// </summary>
    public CarryingTaskType TaskType
    {
        get { return _taskType; }
    }

    private readonly int _taskPriority;

    /// <summary>
    /// 任务优先级
    /// </summary>
    public int TaskPriority
    {
        get { return _taskPriority; }
    }

    private readonly TruckLoadingPosition? _planLoadingPosition;

    /// <summary>
    /// 计划载箱位置
    /// </summary>
    public TruckLoadingPosition? PlanLoadingPosition
    {
        get { return _planLoadingPosition; }
    }

    private readonly DateTime _originateTime;

    /// <summary>
    /// 制单时间
    /// </summary>
    public DateTime OriginateTime
    {
        get { return _originateTime; }
    }

    private readonly CarryingTaskStatus _status;

    /// <summary>
    /// 任务状态
    /// </summary>
    public CarryingTaskStatus Status
    {
        get { return _status; }
    }

    private readonly bool _suspending;

    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool Suspending
    {
        get { return _suspending; }
    }

    private readonly DateTime _suspendingChangeTime;

    /// <summary>
    /// 暂停变更时间
    /// </summary>
    public DateTime SuspendingChangeTime
    {
        get { return _suspendingChangeTime; }
    }

    #region Detail

    [NonSerialized]
    private readonly Dictionary<bool, CarryingContainer?> _containerDict = new Dictionary<bool, CarryingContainer?>(2)
    {
        [true] = null,
        [false] = null
    };

    [Newtonsoft.Json.JsonIgnore]
    private IDictionary<bool, CarryingContainer?> ContainerDict
    {
        get
        {
            if (_containerDict[true] == null)
                foreach (CarryingContainer item in this.FetchDetails<CarryingContainer>())
                    _containerDict[item.IsPlan] = item;
            return _containerDict;
        }
    }

    /// <summary>
    /// 计划运输货柜
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public CarryingContainer PlanContainer
    {
        get { return ContainerDict[true]!; }
    }

    /// <summary>
    /// 实际运输货柜
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public CarryingContainer? ActualContainer
    {
        get { return ContainerDict[false]; }
    }

    /// <summary>
    /// 是大箱
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public bool IsBigSize
    {
        get { return ActualContainer != null ? ActualContainer.IsBigSize : PlanContainer.IsBigSize; }
    }

    [NonSerialized]
    private readonly Dictionary<CarryingTaskOrderType, CarryingTaskOrder> _orderDict = new Dictionary<CarryingTaskOrderType, CarryingTaskOrder>(2);

    [Newtonsoft.Json.JsonIgnore]
    private IDictionary<CarryingTaskOrderType, CarryingTaskOrder> OrderDict
    {
        get
        {
            if (_orderDict.Count == 0)
                foreach (CarryingTaskOrder item in this.FetchDetails<CarryingTaskOrder>())
                    _orderDict[item.OrderType] = item;
            return _orderDict;
        }
    }

    /// <summary>
    /// 装载指令
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public CarryingTaskOrder LoadOrder
    {
        get { return OrderDict[CarryingTaskOrderType.Load]; }
    }

    /// <summary>
    /// 卸载指令
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public CarryingTaskOrder UnloadOrder
    {
        get { return OrderDict[CarryingTaskOrderType.Unload]; }
    }

    #endregion

    #endregion

    #region 方法

    /// <summary>
    /// 变更计划货柜
    /// </summary>
    /// <param name="containerNumber">箱号</param>
    /// <param name="isBigSize">是大箱</param>
    public void ChangePlanContainer(string containerNumber, bool isBigSize)
    {
        ContainerDict[true]!.UpdateSelf(
            CarryingContainer.Set(p => p.ContainerNumber, containerNumber).
                Set(p => p.IsBigSize, isBigSize));
    }

    /// <summary>
    /// 装载货柜
    /// </summary>
    /// <param name="containerNumber">箱号</param>
    /// <param name="isBigSize">是大箱</param>
    public void LoadContainer(string containerNumber, bool isBigSize)
    {
        if (ContainerDict[false] != null)
            ContainerDict[false]!.UpdateSelf(
                CarryingContainer.Set(p => p.ContainerNumber, containerNumber).
                    Set(p => p.IsBigSize, isBigSize));
        else
        {
            CarryingContainer container = this.NewDetail<CarryingContainer>(
                CarryingContainer.Set(p => p.IsPlan, false).
                    Set(p => p.ContainerNumber, containerNumber).
                    Set(p => p.IsBigSize, isBigSize));
            container.InsertSelf();
            ContainerDict[false] = container;
        }
    }

    /// <summary>
    /// 变更装载指令
    /// </summary>
    /// <param name="loadLocation">装载位置</param>
    /// <param name="loadQueueNo">装载排队序号</param>
    /// <param name="loadPlanTime">装载计划时间</param>
    /// <param name="loadCraneNo">装载机械号</param>
    /// <param name="quayCraneProcess">岸桥工艺</param>
    public void ChangeLoadOrder(string loadLocation, long loadQueueNo, DateTime loadPlanTime, string? loadCraneNo, QuayCraneProcess? quayCraneProcess)
    {
        CarryingTaskOrder loadOrder = LoadOrder;
        if (loadOrder.Completed)
            throw new InvalidOperationException($"装载指令已完成执行, 无法替换!");

        loadOrder.UpdateSelf(
            CarryingTaskOrder.Set(p => p.OrderType, CarryingTaskOrderType.Load).
                Set(p => p.LoadUnloadLocation, loadLocation).
                Set(p => p.LoadUnloadQueueNo, loadQueueNo).
                Set(p => p.LoadUnloadPlanTime, loadPlanTime).
                Set(p => p.CraneNo, loadCraneNo).
                Set(p => p.CraneType, TaskType is CarryingTaskType.Discharge or CarryingTaskType.Transship ? CraneType.QuayCrane : CraneType.GantryCrane).
                Set(p => p.QuayCraneProcess, TaskType is CarryingTaskType.Discharge or CarryingTaskType.Transship ? quayCraneProcess : null).
                Set(p => p.NeedTwistLock, false));
    }

    /// <summary>
    /// 变更卸载指令
    /// </summary>
    /// <param name="unloadLocation">卸载位置</param>
    /// <param name="unloadQueueNo">卸载排队序号</param>
    /// <param name="unloadPlanTime">卸载计划时间</param>
    /// <param name="unloadCraneNo">卸载机械号</param>
    /// <param name="quayCraneProcess">岸桥工艺</param>
    /// <param name="needTwistLock">是否需要装卸锁钮</param>
    public void ChangeUnloadOrder(string unloadLocation, long unloadQueueNo, DateTime unloadPlanTime, string? unloadCraneNo, QuayCraneProcess? quayCraneProcess, bool needTwistLock)
    {
        CarryingTaskOrder unloadOrder = UnloadOrder;
        if (unloadOrder.Executing)
            throw new InvalidOperationException($"卸载指令已开始执行, 无法替换!");
        if (unloadOrder.Completed)
            throw new InvalidOperationException($"卸载指令已完成执行, 无法替换!");

        unloadOrder.UpdateSelf(
            CarryingTaskOrder.Set(p => p.OrderType, CarryingTaskOrderType.Unload).
                Set(p => p.LoadUnloadLocation, unloadLocation).
                Set(p => p.LoadUnloadQueueNo, unloadQueueNo).
                Set(p => p.LoadUnloadPlanTime, unloadPlanTime).
                Set(p => p.CraneNo, unloadCraneNo).
                Set(p => p.CraneType, TaskType is CarryingTaskType.Shipment or CarryingTaskType.Transship ? CraneType.QuayCrane : CraneType.GantryCrane).
                Set(p => p.QuayCraneProcess, TaskType is CarryingTaskType.Shipment or CarryingTaskType.Transship ? quayCraneProcess : null).
                Set(p => p.NeedTwistLock, needTwistLock && TaskType != CarryingTaskType.Shift));
    }

    /// <summary>
    /// 执行下一个作业
    /// </summary>
    /// <returns>运输任务作业</returns>
    public CarryingTaskOperation? ExecuteNextOperation()
    {
        Resume();

        return this.Database.ExecuteGet((DbTransaction transaction) =>
        {
            foreach (KeyValuePair<CarryingTaskOrderType, CarryingTaskOrder> kvp in OrderDict)
                if (!kvp.Value.Completed)
                {
                    CarryingTaskOperation result = kvp.Value.ExecuteNextOperation(transaction);
                    switch (kvp.Value.OrderType)
                    {
                        case CarryingTaskOrderType.Load:
                            this.UpdateSelf(transaction, Set(p => p.Status,
                                result.Status == CarryingTaskOperationStatus.LoadUnloaded ? CarryingTaskStatus.Loaded : CarryingTaskStatus.Executing));
                            break;
                        case CarryingTaskOrderType.Unload:
                            this.UpdateSelf(transaction, Set(p => p.Status,
                                result.Status == CarryingTaskOperationStatus.LoadUnloaded ? CarryingTaskStatus.Unloaded : CarryingTaskStatus.Loaded));
                            break;
                    }

                    return result;
                }

            return null;
        });
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    public void Suspend()
    {
        if (Suspending)
            return;

        this.UpdateSelf(Set(p => p.Suspending, true).
            Set(p => p.SuspendingChangeTime, DateTime.Now));
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    public void Resume()
    {
        if (!Suspending)
            return;

        this.UpdateSelf(Set(p => p.Suspending, false).
            Set(p => p.SuspendingChangeTime, DateTime.Now));
    }

    #endregion
}