namespace Bee.CTOS.CollaborativeTruckSchedulingService.DomainServices;

public interface ITrafficManagementService
{
    int DetermineSpeedingViolationInKmh(DateTime entryTimestamp, DateTime exitTimestamp);
    string GetRoadId();
}
