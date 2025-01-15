namespace Bee.CTOS.InternalTruckSchedulingService.DomainServices;

public interface ITaskDispatchService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
