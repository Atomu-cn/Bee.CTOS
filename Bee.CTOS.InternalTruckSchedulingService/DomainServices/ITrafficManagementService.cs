namespace Bee.CTOS.InternalTruckSchedulingService.DomainServices;

public interface ITrafficManagementService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
