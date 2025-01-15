using Phenix.Core.Mapper;
using Phenix.Core.Mapper.Schema;

namespace Bee.CTOS.InternalTruckSchedulingService.Models;

/// <summary>
/// 集卡
/// </summary>
[Sheet("ITS_TRUCK")]
public class Truck : EntityBase<Truck>
{
    /// <summary>
    /// 创建
    /// </summary>
    public static Truck Create(string truckNo, TruckDriveType driveType)
    {
        Truck result = New(Set(p => p.TruckNo, truckNo).
            Set(p => p.DriveType, driveType));
        result.InsertSelf();
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

    private readonly string _truckNo;

    /// <summary>
    /// 集卡编号
    /// </summary>
    public string TruckNo
    {
        get { return _truckNo; }
    }

    private readonly TruckDriveType _driveType;

    /// <summary>
    /// 驾驶类型
    /// </summary>
    public TruckDriveType DriveType
    {
        get { return _driveType; }
    }

    private readonly TruckHealthStatus _healthStatus;

    /// <summary>
    /// 健康状态
    /// </summary>
    public TruckHealthStatus HealthStatus
    {
        get { return _healthStatus; }
    }

    private readonly DateTime _healthStatusChangeTime;

    /// <summary>
    /// 健康状态变更时间
    /// </summary>
    public DateTime HealthStatusChangeTime
    {
        get { return _healthStatusChangeTime; }
    }

    #region Detail

    [NonSerialized]
    private readonly Dictionary<TruckLoadingPosition, TruckCarryingTask?> _taskDict = new Dictionary<TruckLoadingPosition, TruckCarryingTask?>(2)
    {
        [TruckLoadingPosition.Fore] = null,
        [TruckLoadingPosition.Back] = null
    };

    [Newtonsoft.Json.JsonIgnore]
    private IDictionary<TruckLoadingPosition, TruckCarryingTask?> TaskDict
    {
        get
        {
            if (_taskDict[TruckLoadingPosition.Fore] == null)
                foreach (TruckCarryingTask item in this.FetchDetails<TruckCarryingTask>())
                    _taskDict[item.LoadingPosition] = item;
            return _taskDict;
        }
    }

    /// <summary>
    /// 前箱任务
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public CarryingTask? ForeTask
    {
        get { return TaskDict[TruckLoadingPosition.Fore] != null ? TaskDict[TruckLoadingPosition.Fore]?.Task : null; }
    }

    /// <summary>
    /// 后箱任务
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    public CarryingTask? BackTask
    {
        get { return TaskDict[TruckLoadingPosition.Back] != null ? TaskDict[TruckLoadingPosition.Back]?.Task : null; }
    }

    #endregion

    #region 时序数据

    private string? _location;

    /// <summary>
    /// 位置（地图标记位置）
    /// </summary>
    public string? Location
    {
        get { return _location; }
    }

    private double _locationLng;

    /// <summary>
    /// 经度（地图标记位置）
    /// </summary>
    public double LocationLng
    {
        get { return _locationLng; }
    }

    private double _locationLat;

    /// <summary>
    /// 纬度（地图标记位置）
    /// </summary>
    public double LocationLat
    {
        get { return _locationLat; }
    }

    #endregion

    #endregion

    #region 方法

    /// <summary>
    /// 更改驾驶类型
    /// </summary>
    public void ChangeDriveType(TruckDriveType driveType)
    {
        if (DriveType == driveType)
            return;

        this.UpdateSelf(Set(p => p.DriveType, driveType));
    }

    /// <summary>
    /// 更改健康状态
    /// </summary>
    public void ChangeHealthStatus(TruckHealthStatus healthStatus)
    {
        if (HealthStatus == healthStatus)
            return;

        this.UpdateSelf(Set(p => p.HealthStatus, healthStatus).
            Set(p => p.HealthStatusChangeTime, DateTime.Now));
    }

    private bool IsHealthy(bool throwIfUnhealthy = false)
    {
        switch (HealthStatus)
        {
            case TruckHealthStatus.Charging:
                if (throwIfUnhealthy)
                    throw new InvalidOperationException($"集卡{TruckNo}还处在充电或加油状态, 需先恢复到正常状态后才能继续操作!");
                return false;
            case TruckHealthStatus.Maintaining:
                if (throwIfUnhealthy)
                    throw new InvalidOperationException($"集卡{TruckNo}还处在维修或保养状态, 需先恢复到正常状态后才能继续操作!");
                return false;
            default:
                return true;
        }
    }

    /// <summary>
    /// 移动到
    /// </summary>
    /// <param name="location">位置（地图标记位置）</param>
    /// <param name="locationLng">经度（地图标记位置）</param>
    /// <param name="locationLat">纬度（地图标记位置）</param>
    public void MoveTo(string location, double locationLng, double locationLat)
    {
        _location = location;
        _locationLng = locationLng;
        _locationLat = locationLat;
    }

    /// <summary>
    /// 接受新任务
    /// </summary>
    /// <param name="task">运输任务</param>
    /// <returns>确认载箱位置</returns>
    public TruckCarryingTask NewTask(CarryingTask task)
    {
        if (!IsHealthy(true))
            return null;

        CarryingTask? foreTask = ForeTask;
        CarryingTask? backTask = BackTask;
        if (foreTask != null)
            if (foreTask.TaskId == task.TaskId)
                throw new InvalidOperationException($"本集卡{TruckNo}前箱位置已有相同{task.TaskId}任务，请使用ForeTask的相关函数更新任务!");
            else if (task.PlanLoadingPosition == TruckLoadingPosition.Fore && foreTask.Status != CarryingTaskStatus.Unloaded)
                throw new InvalidOperationException($"本集卡{TruckNo}前箱位置的{foreTask.TaskId}任务还未完成，无法承接新任务!");
        if (backTask != null)
            if (backTask.TaskId == task.TaskId)
                throw new InvalidOperationException($"本集卡{TruckNo}后箱位置已有相同{task.TaskId}任务，请使用BackTask的相关函数更新任务!");
            else if (task.PlanLoadingPosition == TruckLoadingPosition.Fore && backTask.Status != CarryingTaskStatus.Unloaded)
                throw new InvalidOperationException($"本集卡{TruckNo}后箱位置的{backTask.TaskId}任务还未完成，无法承接新任务!");

        TruckLoadingPosition? position = task.PlanLoadingPosition;
        if (position == null)
            foreach (KeyValuePair<TruckLoadingPosition, TruckCarryingTask?> kvp in TaskDict)
                if (kvp.Value == null || kvp.Value.Task.Status == CarryingTaskStatus.Unloaded)
                {
                    position = kvp.Key;
                    break;
                }

        if (position == null)
            throw new InvalidOperationException($"本集卡{TruckNo}暂无载箱位置可供安排任务!");

        TruckCarryingTask result = this.NewDetail<TruckCarryingTask>(
            TruckCarryingTask.Set(p => p.TerminalNo, task.TerminalNo).
                Set(p => p.TruckPoolsNo, task.TruckPoolsNo).
                Set(p => p.TaskId, task.TaskId).
                Set(p => p.LoadingPosition, position).
                Set(p => p.OriginateTime, DateTime.Now));
        result.InsertSelf();
        TaskDict[position.Value] = result;
        return result;
    }

    /// <summary>
    /// 取消任务（含强制取消）
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <returns>确认载箱位置</returns>
    public TruckLoadingPosition CancelTask(long taskId)
    {
        TruckLoadingPosition? position = null;
        foreach (KeyValuePair<TruckLoadingPosition, TruckCarryingTask?> kvp in TaskDict)
            if (kvp.Value != null && kvp.Value.TaskId == taskId)
            {
                position = kvp.Key;
                break;
            }

        if (position == null)
            throw new InvalidOperationException($"本集卡{TruckNo}不存在{taskId}任务，无从取消!");

        this.DeleteDetails<TruckCarryingTask>(p => p.LoadingPosition == position);
        TaskDict[position.Value] = null;
        return position.Value;
    }

    #endregion
}