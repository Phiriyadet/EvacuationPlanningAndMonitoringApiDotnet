using Evacuation.Application.DTOs.Vehicle;
using Evacuation.Application.Mappers;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Evacuation.Shared.Result;
using Evacuation.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;

        public VehicleService(IVehicleRepository vehicleRepo)
        {
            _vehicleRepo = vehicleRepo;
        }

        public async Task<OperationResult<VehicleDto>> AddVehicleAsync(CreateVehicleDto createDto)
        {
            try 
            {
                var errors = ValidationHelper.ValidateObject(createDto);
                if (errors.Any())
                {
                    return OperationResult<VehicleDto>.Fail(errors, "Validation errors occurred");
                }

                var vehicle = createDto.CreateToEntity();
                await _vehicleRepo.AddAsync(vehicle);
                return OperationResult<VehicleDto>.Ok(vehicle.ToDto());
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<VehicleDto>.Fail("Database update error occurred while adding the vehicle", null, dbEx);
            }
            catch (ArgumentException argEx)
            {
                return OperationResult<VehicleDto>.Fail($"Error generating vehicle ID: {argEx.Message}", null, argEx);
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleDto>.Fail("Error adding vehicle", null, ex);
            }

        }

        public async Task<OperationResult<bool>> DeleteVehicleAsync(int vehicleId)
        {
            return await _vehicleRepo.DeleteAsync(vehicleId)
                ? OperationResult<bool>.Ok(true, $"Vehicle with ID {vehicleId} deleted successfully")   
                : OperationResult<bool>.Fail("Vehicle not found", false);
        }

        public async Task<OperationResult<IEnumerable<VehicleDto>>> GetAllVehiclesAsync()
        {
            try 
            {
                var vehicles = await _vehicleRepo.GetAllAsync();
                if (vehicles == null || !vehicles.Any())
                {
                    return OperationResult<IEnumerable<VehicleDto>>.Fail("No vehicles found", null);
                }
                return OperationResult<IEnumerable<VehicleDto>>.Ok(vehicles.ToDto());
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<VehicleDto>>.Fail("Error retrieving vehicles", null, ex);
            }
        }

        public async Task<OperationResult<VehicleDto>> GetVehicleByIdAsync(int vehicleId)
        {
            try 
            {
                var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    return OperationResult<VehicleDto>.Fail("Vehicle not found", null);
                }
                return OperationResult<VehicleDto>.Ok(vehicle.ToDto());
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleDto>.Fail("Error retrieving vehicle", null, ex);
            }
        }

        public async Task<OperationResult<VehicleDto>> UpdateVehicleAsync(int Id, UpdateVehicleDto updateDto)
        {
            try 
            {
                var errors = ValidationHelper.ValidateObject(updateDto);
                if (errors.Any())
                {
                    return OperationResult<VehicleDto>.Fail(errors, "Validation errors occurred");
                }

                var existingVehicle = await _vehicleRepo.GetByIdAsync(Id);
                if (existingVehicle == null)
                {
                    return OperationResult<VehicleDto>.Fail("Vehicle not found", null);
                }

                var vehicle = await _vehicleRepo.UpdateAsync(Id, updateDto.UpdateToEntity(existingVehicle));
                if (vehicle == null)
                {
                    return OperationResult<VehicleDto>.Fail("Vehicle not found", null);
                }

                return OperationResult<VehicleDto>.Ok(vehicle.ToDto());
            }
            catch (Exception ex)
            {
                return OperationResult<VehicleDto>.Fail("Error updating vehicle", null, ex);
            }
        }
    }
}
