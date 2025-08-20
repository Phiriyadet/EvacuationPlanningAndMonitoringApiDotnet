using Evacuation.Application.DTOs.Location;
using Evacuation.Domain.ValueObjects;

namespace Evacuation.Application.Mappers
{
    public static class LocationMapper
    {
        public static LocationCoordinatesDto ToDto(this LocationCoordinates location)
        {
            return new LocationCoordinatesDto
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            };
        }
        
        public static LocationCoordinates ToEntity(this LocationCoordinatesDto locationDto)
        {
            return new LocationCoordinates
            (
                locationDto.Latitude,
                locationDto.Longitude
            );
        }
    }
      
}
