using Evacuation.Application.DTOs.Location;
using Evacuation.Domain.Enums;

namespace Evacuation.Application.DTOs.Vehicle
{
    public class VehicleDto
    {
        public string VehicleId { get; set; } = string.Empty;

        public VehicleType Type { get; set; } = VehicleType.Unknown;
        public int Capacity { get; set; }
        public int Speed { get; set; } // Speed in km/h
        public bool IsAvailable { get; set; }// Indicates if the vehicle is available for use

        public LocationCoordinatesDto Location { get; set; } = new LocationCoordinatesDto();
    }
}
