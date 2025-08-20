using Evacuation.Application.DTOs.Status;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class StatusMapper
    {
        public static StatusDto ToDto(this Status status, string zoneId, string lastVehicleUsedId)
        {
            return new StatusDto
            {
                StatusId = status.BusinessId,
                ZoneId = zoneId,
                TotalEvacuatedPeople = status.TotalEvacuatedPeople,
                RemainingPeople = status.RemainingPeople,
                LastVehicleUsedId = lastVehicleUsedId
            };
        }

        public static IEnumerable<StatusDto> ToDto(this IEnumerable<Status> statuses,
        IDictionary<int, string> zoneIdMap, 
        IDictionary<int, string> vehicleIdMap)
        {
            return statuses.Select(s => s.ToDto(
                zoneIdMap[s.ZoneId],
                vehicleIdMap[s.LastVehicleIdUsed]
            ));
        }

        public static Status CreateToEntity(this CreateStatusDto createDto)
        {
            return new Status
            (
                createDto.ZoneId,
                createDto.TotalEvacuatedPeople,
                createDto.RemainingPeople,
                createDto.LastVehicleUsedId
            );
        }

        public static Status UpdateToEntity(this UpdateStatusDto updateDto, Status existingStatus)
        {
            existingStatus.Update
            (
                updateDto.TotalEvacuatedPeople,
                updateDto.RemainingPeople,
                updateDto.LastVehicleUsedId
            );
            return existingStatus;
        }
    }
}
