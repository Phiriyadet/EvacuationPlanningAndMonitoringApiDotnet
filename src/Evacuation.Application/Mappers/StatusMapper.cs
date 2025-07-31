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
                ZoneId = status.ZoneId,
                TotalPeopleEvacuated = status.TotalPeopleEvacuated,
                RemainingPeople = status.RemainingPeople,
                LastVehicleId = status.LastVehicleId
            };
        }

        public static IEnumerable<StatusDto> ToDto(this IEnumerable<Status> statuses)
        {
            return statuses.Select(s => s.ToDto());
        }

        public static Status CreateToEntity(this CreateStatusDto createDto)
        {
            return new Status
            {
                ZoneId = createDto.ZoneId,
                TotalPeopleEvacuated = createDto.TotalPeopleEvacuated,
                RemainingPeople = createDto.RemainingPeople,
                LastVehicleId = createDto.LastVehicleId
            };
        }

        public static Status UpdateToEntity(this UpdateStatusDto updateDto)
        {
            return new Status
            {
                TotalPeopleEvacuated = updateDto.TotalPeopleEvacuated,
                RemainingPeople = updateDto.RemainingPeople,
                LastVehicleId = updateDto.LastVehicleId
            };
        }
    }
}
