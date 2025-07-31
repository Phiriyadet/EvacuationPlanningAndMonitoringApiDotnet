using Evacuation.Domain.Enums;
using Evacuation.Domain.ValueObjects;

namespace Evacuation.Domain.Entities
{
    public class Vehicle
    {
        public string VehicleId { get; set; } = string.Empty;

        public VehicleType Type { get; set; } = VehicleType.Unknown;
        public int Capacity { get; set; }
        public int Speed { get; set; } // Speed in km/h
        public bool IsAvailable { get; set; } = true; // Indicates if the vehicle is available for use

        public LocationCoordinates LocationCoordinates { get; set; } = new LocationCoordinates();

        //private Vehicle() { } // cannot be instantiated directly

    }
}
