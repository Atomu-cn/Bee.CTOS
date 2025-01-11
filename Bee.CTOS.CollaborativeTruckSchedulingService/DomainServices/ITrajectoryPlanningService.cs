namespace Bee.CTOS.CollaborativeTruckSchedulingService.DomainServices;

public interface ITrajectoryPlanningService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
