using Evacuation.Application.DTOs.Vehicle;
using Evacuation.Application.Mappers;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Evacuation.Shared.Result;
using Evacuation.Shared.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Evacuation.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository vehicleRepo,ILogger<VehicleService> logger)
        {
            _vehicleRepo = vehicleRepo;
            _logger = logger;
        }

        public async Task<OperationResult<VehicleDto>> AddVehicleAsync(CreateVehicleDto createDto)
        {
            _logger.LogInformation("At Time {Time}, AddVehicleAsync called", DateTime.UtcNow);
            try 
            {
                var errors = ValidationHelper.ValidateObject(createDto);
                if (errors.Any())
                {
                    _logger.LogWarning("Validation errors occurred while adding vehicle: {Errors}", string.Join(", ", errors));
                    return OperationResult<VehicleDto>.Fail(errors, "Validation errors occurred");
                }

                var vehicle = createDto.CreateToEntity();
                await _vehicleRepo.AddAsync(vehicle);

                _logger.LogInformation("Vehicle with ID {VehicleId} added successfully", vehicle.Id);
                return OperationResult<VehicleDto>.Ok(vehicle.ToDto());
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error occurred while adding vehicle");
                return OperationResult<VehicleDto>.Fail("Database update error occurred while adding the vehicle", null, dbEx);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogError(argEx, "Error generating vehicle ID");
                return OperationResult<VehicleDto>.Fail($"Error generating vehicle ID: {argEx.Message}", null, argEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding vehicle");
                return OperationResult<VehicleDto>.Fail("Error adding vehicle", null, ex);
            }

        }

        public async Task<OperationResult<bool>> DeleteVehicleAsync(int vehicleId)
        {
            _logger.LogInformation("At Time {Time}, DeleteVehicleAsync called for Vehicle ID {VehicleId}", DateTime.UtcNow, vehicleId);
            try 
            {
                var deleted = await _vehicleRepo.DeleteAsync(vehicleId);
                if (deleted)
                {
                    _logger.LogInformation("Vehicle with ID {VehicleId} deleted successfully", vehicleId);
                    return OperationResult<bool>.Ok(true, $"Vehicle with ID {vehicleId} deleted successfully");
                }
                _logger.LogInformation("Vehicle with ID {VehicleId} not found", vehicleId);
                return OperationResult<bool>.Fail("Vehicle not found", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting vehicle with ID {VehicleId}", vehicleId);
                return OperationResult<bool>.Fail("Error deleting vehicle", false, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<VehicleDto>>> GetAllVehiclesAsync()
        {
            _logger.LogInformation("At Time {Time}, GetAllVehiclesAsync called", DateTime.UtcNow);
            try 
            {
                var vehicles = await _vehicleRepo.GetAllAsync();
                if (vehicles == null || !vehicles.Any())
                {
                    _logger.LogInformation("No vehicles found in the database");
                    return OperationResult<IEnumerable<VehicleDto>>.Fail("No vehicles found", null);
                }

                _logger.LogInformation("Retrieved {Count} vehicles from the database", vehicles.Count());
                return OperationResult<IEnumerable<VehicleDto>>.Ok(vehicles.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving vehicles");
                return OperationResult<IEnumerable<VehicleDto>>.Fail("Error retrieving vehicles", null, ex);
            }
        }

        public async Task<OperationResult<VehicleDto>> GetVehicleByIdAsync(int vehicleId)
        {
            _logger.LogInformation("At Time {Time}, GetVehicleByIdAsync called for Vehicle ID {VehicleId}", DateTime.UtcNow, vehicleId);
            try 
            {
                var vehicle = await _vehicleRepo.GetByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    _logger.LogInformation("Vehicle with ID {VehicleId} not found", vehicleId);
                    return OperationResult<VehicleDto>.Fail("Vehicle not found", null);
                }

                _logger.LogInformation("Vehicle with ID {VehicleId} retrieved successfully", vehicleId);
                return OperationResult<VehicleDto>.Ok(vehicle.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving vehicle with ID {VehicleId}", vehicleId);
                return OperationResult<VehicleDto>.Fail("Error retrieving vehicle", null, ex);
            }
        }

        public async Task<OperationResult<VehicleDto>> UpdateVehicleAsync(int Id, UpdateVehicleDto updateDto)
        {
            _logger.LogInformation("At Time {Time}, UpdateVehicleAsync called for Vehicle ID {VehicleId}", DateTime.UtcNow, Id);
            try 
            {
                var errors = ValidationHelper.ValidateObject(updateDto);
                if (errors.Any())
                {
                    _logger.LogWarning("Validation errors occurred while updating vehicle: {Errors}", string.Join(", ", errors));
                    return OperationResult<VehicleDto>.Fail(errors, "Validation errors occurred");
                }

                var existingVehicle = await _vehicleRepo.GetByIdAsync(Id);
                if (existingVehicle == null)
                {
                    _logger.LogInformation("Vehicle with ID {VehicleId} not found", Id);
                    return OperationResult<VehicleDto>.Fail("Vehicle not found", null);
                }

                var vehicle = await _vehicleRepo.UpdateAsync(Id, updateDto.UpdateToEntity(existingVehicle));
                if (vehicle == null)
                {
                    _logger.LogInformation("Vehicle with ID {VehicleId} not found after update attempt", Id);
                    return OperationResult<VehicleDto>.Fail("Vehicle not found", null);
                }

                _logger.LogInformation("Vehicle with ID {VehicleId} updated successfully", Id);
                return OperationResult<VehicleDto>.Ok(vehicle.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating vehicle with ID {VehicleId}", Id);
                return OperationResult<VehicleDto>.Fail("Error updating vehicle", null, ex);
            }
        }
    }
}
