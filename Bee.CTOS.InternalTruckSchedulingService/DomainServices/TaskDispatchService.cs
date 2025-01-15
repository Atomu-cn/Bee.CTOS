namespace Bee.CTOS.InternalTruckSchedulingService.DomainServices;

/// <summary>
/// 任务派送（从全局和更有远见的角度优化资源利用率，找到任务和集卡之间的最佳匹配）
/// </summary>
public class TaskDispatchService : ITaskDispatchService
{
    private readonly string _roadId;
    private readonly int _sectionLengthInKm;
    private readonly int _maxAllowedSpeedInKmh;
    private readonly int _legalCorrectionInKmh;

    public TaskDispatchService(string roadId, int sectionLengthInKm, int maxAllowedSpeedInKmh, int legalCorrectionInKmh)
    {
        _roadId = roadId;
        _sectionLengthInKm = sectionLengthInKm;
        _maxAllowedSpeedInKmh = maxAllowedSpeedInKmh;
        _legalCorrectionInKmh = legalCorrectionInKmh;
    }

    public int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp)
    {
        double elapsedMinutes = exitTimestamp.Subtract(entryTimestamp).TotalSeconds; // 1 sec. == 1 min. in simulation
        double avgSpeedInKmh = Math.Round((_sectionLengthInKm / elapsedMinutes) * 60);
        int violation = Convert.ToInt32(avgSpeedInKmh - _maxAllowedSpeedInKmh - _legalCorrectionInKmh);
        return violation;
    }

    public string GetRoadId()
    {
        return _roadId;
    }
}
