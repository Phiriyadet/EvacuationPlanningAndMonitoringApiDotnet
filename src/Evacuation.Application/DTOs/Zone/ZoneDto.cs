using Evacuation.Application.DTOs.Location;

namespace Evacuation.Application.DTOs.Zone
{
    public class ZoneDto
    {
        public string ZoneId { get; set; } = string.Empty;
        public int NumberOfPeople { get; set; }
        public int UrgencyLevel { get; set; }
        public LocationCoordinatesDto Location { get; set; } = new LocationCoordinatesDto();
    }
}
