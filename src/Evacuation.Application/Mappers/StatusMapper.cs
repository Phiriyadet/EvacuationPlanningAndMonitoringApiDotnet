using Evacuation.Application.DTOs.Status;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class StatusMapper
    {
        public static StatusDto ToDto(this Status status)
        {
            return new StatusDto
            { 
                StatusId = status.BusinessId,
                ZoneId = status.Zone.BusinessId,
                TotalEvacuatedPeople = status.TotalEvacuatedPeople,
                RemainingPeople = status.RemainingPeople,
                LastVehicleUsedId = status.Vehicle.BusinessId
            };
        }

        public static IEnumerable<StatusDto> ToDto(this IEnumerable<Status> statuses)
        {
            return statuses.Select(s => s.ToDto());
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
