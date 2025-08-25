using Evacuation.Application.DTOs.Zone;
using Evacuation.Application.Mappers;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Evacuation.Shared.Result;
using Evacuation.Shared.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Evacuation.Application.Services
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepo;
        private readonly ILogger<ZoneService> _logger;
        public ZoneService(IZoneRepository zoneRepo, ILogger<ZoneService> logger)
        {
            _zoneRepo = zoneRepo;
            _logger = logger;
        }
        public async Task<OperationResult<ZoneDto>> AddZoneAsync(CreateZoneDto createDto)
        {
            _logger.LogInformation("At Time {Time}, AddZoneAsync called", DateTime.UtcNow);
            try
            {
                var errors = ValidationHelper.ValidateObject(createDto);
                if (errors.Any())
                {
                    _logger.LogWarning("Validation errors occurred while adding zone: {Errors}", string.Join(", ", errors));
                    return OperationResult<ZoneDto>.Fail(errors, "Validation errors occurred");
                }

                var zone = createDto.CreateToEntity();
                await _zoneRepo.AddAsync(zone);

                _logger.LogInformation("Zone with ID {ZoneId} added successfully", zone.Id);
                return OperationResult<ZoneDto>.Ok(zone.ToDto());
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error occurred while adding zone");
                return OperationResult<ZoneDto>.Fail("Database update error occurred while adding the zone", null, dbEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding zone");
                return OperationResult<ZoneDto>.Fail("Error adding zone", null, ex);
            }
        }

        public async Task<OperationResult<bool>> DeleteZoneAsync(int zoneId)
        {
            _logger.LogWarning("At Time {Time}, DeleteZoneAsync called for Zone ID {ZoneId}", DateTime.UtcNow, zoneId);
            try 
            {
                var deleted = await _zoneRepo.DeleteAsync(zoneId);
                if (deleted)
                {
                    _logger.LogInformation("Zone with ID {ZoneId} deleted successfully", zoneId);
                    return OperationResult<bool>.Ok(true, $"Zone with ID {zoneId} deleted successfully");
                }

                _logger.LogWarning("Zone with ID {ZoneId} not found for deletion", zoneId);
                return OperationResult<bool>.Fail("Zone not found", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting zone with ID {ZoneId}", zoneId);
                return OperationResult<bool>.Fail("Error deleting zone", false, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<ZoneDto>>> GetAllZonesAsync()
        {
            _logger.LogInformation("At Time {Time}, GetAllZonesAsync called", DateTime.UtcNow);
            try 
            {
                var zones = await _zoneRepo.GetAllAsync();
                if (zones == null || !zones.Any())
                {
                    _logger.LogInformation("No zones found in the database");
                    return OperationResult<IEnumerable<ZoneDto>>.Fail("No zones found", null);
                }

                _logger.LogInformation("Retrieved {Count} zones from the database", zones.Count());
                return OperationResult<IEnumerable<ZoneDto>>.Ok(zones.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving zones");
                return OperationResult<IEnumerable<ZoneDto>>.Fail("Error retrieving zones", null, ex);
            }
        }

        public async Task<OperationResult<ZoneDto>> GetZoneByIdAsync(int zoneId)
        {
            _logger.LogInformation("At Time {Time}, GetZoneByIdAsync called for Zone ID {ZoneId}", DateTime.UtcNow, zoneId);
            try 
            {
                var zone = await _zoneRepo.GetByIdAsync(zoneId);
                if (zone == null)
                {
                    _logger.LogInformation("Zone with ID {ZoneId} not found", zoneId);
                    return OperationResult<ZoneDto>.Fail("Zone not found", null);
                }

                _logger.LogInformation("Zone with ID {ZoneId} retrieved successfully", zoneId);
                return OperationResult<ZoneDto>.Ok(zone.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving zone with ID {ZoneId}", zoneId);
                return OperationResult<ZoneDto>.Fail("Error retrieving zone", null, ex);
            }
        }

        public async Task<OperationResult<ZoneDto>> UpdateZoneAsync(int Id, UpdateZoneDto updateDto)
        {
            _logger.LogInformation("At Time {Time}, UpdateZoneAsync called for Zone ID {ZoneId}", DateTime.UtcNow, Id);
            try 
            {
                var errors = ValidationHelper.ValidateObject(updateDto);
                if (errors.Any())
                {
                    _logger.LogWarning("Validation errors occurred while updating zone: {Errors}", string.Join(", ", errors));
                    return OperationResult<ZoneDto>.Fail(errors, "Validation errors occurred");
                }

                var existingZone = await _zoneRepo.GetByIdAsync(Id);
                if (existingZone == null)
                {
                    _logger.LogInformation("Zone with ID {ZoneId} not found for update", Id);
                    return OperationResult<ZoneDto>.Fail($"Zone not found");
                }

                var zone = await _zoneRepo.UpdateAsync(Id, updateDto.UpdateToEntity(existingZone));
                if (zone == null)
                {
                    _logger.LogWarning("Failed to update zone with ID {ZoneId}", Id);
                    return OperationResult<ZoneDto>.Fail("Zone not found");
                }

                _logger.LogInformation("Zone with ID {ZoneId} updated successfully", zone.Id);
                return OperationResult<ZoneDto>.Ok(zone.ToDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating zone with ID {ZoneId}", Id);
                return OperationResult<ZoneDto>.Fail("Error updating zone", null, ex);
            }
        }
    }
}
