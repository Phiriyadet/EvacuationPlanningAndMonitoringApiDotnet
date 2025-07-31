using Evacuation.Application.DTOs.Zone;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class ZoneMapper
    {
        public static ZoneDto ToDto(this Zone zone)
        {
            return new ZoneDto
            {
                ZoneId = zone.ZoneId,
                NumberOfPeople = zone.NumberOfPeople,
                UrgencyLevel = zone.UrgencyLevel,
                Location = zone.LocationCoordinates.ToDto()
            };
        }

        public static IEnumerable<ZoneDto> ToDto(this IEnumerable<Zone> zones)
        {
            return zones.Select(z => z.ToDto());
        }

        public static Zone CreateToEntity(this CreateZoneDto createDto, string id)
        {
            return new Zone
            {
                ZoneId = id,
                NumberOfPeople = createDto.NumberOfPeople,
                UrgencyLevel = createDto.UrgencyLevel,
                LocationCoordinates = createDto.Location.ToEntity()
            };
        }

        public static Zone UpdateToEntity(this UpdateZoneDto updateDto)
        {
            return new Zone
            {
                NumberOfPeople = updateDto.NumberOfPeople,
                UrgencyLevel = updateDto.UrgencyLevel,
                LocationCoordinates = updateDto.Location.ToEntity()
            };
        }

    }
}
