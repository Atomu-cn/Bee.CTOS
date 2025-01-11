namespace Bee.CTOS.CollaborativeTruckSchedulingService.DomainServices;

/// <summary>
/// 路径规划（连接起点位置和终点位置的序列点或曲线称之为路径，构成路径的策略称之为路径规划）
/// </summary>
public class PathPlanningService : IPathPlanningService
{
    private readonly string _roadId;
    private readonly int _sectionLengthInKm;
    private readonly int _maxAllowedSpeedInKmh;
    private readonly int _legalCorrectionInKmh;

    public PathPlanningService(string roadId, int sectionLengthInKm, int maxAllowedSpeedInKmh, int legalCorrectionInKmh)
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
