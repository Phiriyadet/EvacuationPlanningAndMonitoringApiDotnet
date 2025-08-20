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
                ZoneId = zone.BusinessId,
                NumberOfPeople = zone.NumberOfPeople,
                UrgencyLevel = zone.UrgencyLevel,
                Location = zone.LocationCoordinates.ToDto()
            };
        }

        public static IEnumerable<ZoneDto> ToDto(this IEnumerable<Zone> zones)
        {
            return zones.Select(z => z.ToDto());
        }

        public static Zone CreateToEntity(this CreateZoneDto createDto)
        {
            return new Zone
            (
                createDto.NumberOfPeople,
                createDto.UrgencyLevel,
                createDto.Location.ToEntity()
            );
        }

        public static Zone UpdateToEntity(this UpdateZoneDto updateDto, Zone existingZone)
        {
            existingZone.Update
            (
                updateDto.NumberOfPeople,
                updateDto.UrgencyLevel,
                updateDto.Location.ToEntity()
            );
            return existingZone;
        }

    }
}
