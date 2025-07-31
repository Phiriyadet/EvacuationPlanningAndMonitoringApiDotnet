using Evacuation.Application.DTOs.Location;
using Evacuation.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Evacuation.Application.DTOs.Vehicle
{
    public class UpdateVehicleDto
    {
        
        [EnumDataType(typeof(VehicleType), ErrorMessage = "Invalid vehicle type.")]
        public VehicleType Type { get; set; } = VehicleType.Unknown;

        [Range(0, int.MaxValue, ErrorMessage = "Capacity must be a non-negative integer.")]
        public int Capacity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Speed must be a non-negative integer.")]
        public int Speed { get; set; } // Speed in km/h

        public LocationCoordinatesDto Location { get; set; } = new LocationCoordinatesDto();
    }
}
