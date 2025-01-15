namespace Bee.CTOS.InternalTruckSchedulingService.DomainServices;

public interface ITrajectoryPlanningService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
