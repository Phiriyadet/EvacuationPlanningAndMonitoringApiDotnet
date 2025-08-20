using Evacuation.Application.DTOs.Zone;
using Evacuation.Shared.Result;

namespace Evacuation.Application.Services.Interfaces
{
    public interface IZoneService
    {
        Task<OperationResult<IEnumerable<ZoneDto>>> GetAllZonesAsync();
        Task<OperationResult<ZoneDto>> GetZoneByIdAsync(int zoneId);
        Task<OperationResult<ZoneDto>> UpdateZoneAsync(int Id, UpdateZoneDto updateDto);
        Task<OperationResult<bool>> DeleteZoneAsync(int zoneId);
        Task<OperationResult<ZoneDto>> AddZoneAsync(CreateZoneDto createDto);
    }
}
