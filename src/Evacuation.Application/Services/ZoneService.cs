using Evacuation.Application.DTOs.Zone;
using Evacuation.Application.Mappers;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Evacuation.Shared.Result;
using Evacuation.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Application.Services
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepo;
        public ZoneService(IZoneRepository zoneRepo)
        {
            _zoneRepo = zoneRepo;
        }
        public async Task<OperationResult<ZoneDto>> AddZoneAsync(CreateZoneDto createDto)
        {
            try
            {
                var errors = ValidationHelper.ValidateObject(createDto);
                if (errors.Any())
                {
                    return OperationResult<ZoneDto>.Fail(errors, "Validation errors occurred");
                }

                var zone = createDto.CreateToEntity();
                await _zoneRepo.AddAsync(zone);
                return OperationResult<ZoneDto>.Ok(zone.ToDto());
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<ZoneDto>.Fail("Database update error occurred while adding the zone", null, dbEx);
            }
            catch (ArgumentException argEx)
            {
                return OperationResult<ZoneDto>.Fail($"Error generating zone ID: {argEx.Message}", null, argEx);
            }
            catch (Exception ex)
            {
                return OperationResult<ZoneDto>.Fail("Error adding zone", null, ex);
            }
        }

        public async Task<OperationResult<bool>> DeleteZoneAsync(int zoneId)
        {
            try 
            {
                var deleted = await _zoneRepo.DeleteAsync(zoneId);
                if (deleted)
                {
                    return OperationResult<bool>.Ok(true, $"Zone with ID {zoneId} deleted successfully");
                }
                return OperationResult<bool>.Fail("Zone not found", false);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Fail("Error deleting zone", false, ex);
            }
        }

        public async Task<OperationResult<IEnumerable<ZoneDto>>> GetAllZonesAsync()
        {
            try 
            {
                var zones = await _zoneRepo.GetAllAsync();
                if (zones == null || !zones.Any())
                {
                    return OperationResult<IEnumerable<ZoneDto>>.Fail("No zones found", null);
                }
                return OperationResult<IEnumerable<ZoneDto>>.Ok(zones.ToDto());
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<ZoneDto>>.Fail("Error retrieving zones", null, ex);
            }
        }

        public async Task<OperationResult<ZoneDto>> GetZoneByIdAsync(int zoneId)
        {
            try 
            {
                var zone = await _zoneRepo.GetByIdAsync(zoneId);
                if (zone == null)
                {
                    return OperationResult<ZoneDto>.Fail("Zone not found", null);
                }
                return OperationResult<ZoneDto>.Ok(zone.ToDto());
            }
            catch (Exception ex)
            {
                return OperationResult<ZoneDto>.Fail("Error retrieving zone", null, ex);
            }
        }

        public async Task<OperationResult<ZoneDto>> UpdateZoneAsync(int Id, UpdateZoneDto updateDto)
        {
            try 
            {
                var errors = ValidationHelper.ValidateObject(updateDto);
                if (errors.Any())
                {
                    return OperationResult<ZoneDto>.Fail(errors, "Validation errors occurred");
                }

                var existingZone = await _zoneRepo.GetByIdAsync(Id);
                if (existingZone == null)
                {
                    return OperationResult<ZoneDto>.Fail($"Zone not found");
                }

                var zone = await _zoneRepo.UpdateAsync(Id, updateDto.UpdateToEntity(existingZone));
                if (zone == null)
                {
                    return OperationResult<ZoneDto>.Fail("Zone not found");
                }

                return OperationResult<ZoneDto>.Ok(zone.ToDto());
            }
            catch (Exception ex)
            {
                return OperationResult<ZoneDto>.Fail("Error updating zone", null, ex);
            }
        }
    }
}
