using Evacuation.Application.DTOs.Vehicle;
using Evacuation.Domain.Entities;

namespace Evacuation.Application.Mappers
{
    public static class VehicleMapper
    {
        public static VehicleDto ToDto(this Vehicle vehicle)
        {
            return new VehicleDto
            {
                VehicleId = vehicle.VehicleId,
                Type = vehicle.Type,
                Capacity = vehicle.Capacity,
                Speed = vehicle.Speed,
                IsAvailable = vehicle.IsAvailable,
                Location = vehicle.LocationCoordinates.ToDto()
            };
        }

        public static IEnumerable<VehicleDto> ToDto(this IEnumerable<Vehicle> vehicles)
        {
            return vehicles.Select(v => v.ToDto());
        }

        public static Vehicle CreateToEntity(this CreateVehicleDto createDto, string Id)
        {
            return new Vehicle
            {
                VehicleId = Id,
                Type = createDto.Type,
                Capacity = createDto.Capacity,
                Speed = createDto.Speed,
                IsAvailable = true, // Default to available when created
                LocationCoordinates = createDto.Location.ToEntity()
            };
        }

        public static Vehicle UpdateToEntity(this UpdateVehicleDto updateDto)
        {
            return new Vehicle
            {
                Type = updateDto.Type,
                Capacity = updateDto.Capacity,
                Speed = updateDto.Speed,
                LocationCoordinates = updateDto.Location.ToEntity()
            };
        }
    }
}
