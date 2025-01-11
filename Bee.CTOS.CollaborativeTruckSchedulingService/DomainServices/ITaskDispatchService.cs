namespace Bee.CTOS.CollaborativeTruckSchedulingService.DomainServices;

public interface ITaskDispatchService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
