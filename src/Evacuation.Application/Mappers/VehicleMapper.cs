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
                VehicleId = vehicle.BusinessId,
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

        public static Vehicle CreateToEntity(this CreateVehicleDto createDto)
        {
            return new Vehicle
            (
                createDto.Type,
                createDto.Capacity,
                createDto.Speed,
                true, // Default to available when creating
                createDto.Location.ToEntity()
            );
        }

        public static Vehicle UpdateToEntity(this UpdateVehicleDto updateDto, Vehicle existingVehicle)
        {
            existingVehicle.Update
            (
                updateDto.Type,
                updateDto.Capacity,
                updateDto.Speed,
                updateDto.Location.ToEntity()
            );
            return existingVehicle;
        }
    }
}
