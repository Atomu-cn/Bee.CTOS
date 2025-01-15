namespace Bee.CTOS.InternalTruckSchedulingService.DomainServices;

public interface IPathPlanningService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
