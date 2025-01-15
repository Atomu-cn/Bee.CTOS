namespace Bee.CTOS.InternalTruckSchedulingService.Events;

/// <summary>
/// 运输任务作业
/// </summary>
public record struct CarryingTaskOperation(
    long TaskId, //任务ID
    string OperationStatus, //作业状态（可与 CarryingTaskOperationStatus 常数值互转）
    string TruckNo, //集卡号
    string LoadingPosition, //运载位置（可与 LoadingPosition 常数值互转）
    bool ShoreShiftUp, //已上档
    DateTime Timestamp);