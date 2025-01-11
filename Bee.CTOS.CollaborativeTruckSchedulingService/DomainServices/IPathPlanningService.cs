namespace Bee.CTOS.CollaborativeTruckSchedulingService.DomainServices;

public interface IPathPlanningService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
