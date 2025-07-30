using Evacuation.Application.DTOs.Zone;
using Evacuation.Shared.Result;

namespace Evacuation.Application.Interfaces.IServices
{
    public interface IZoneService
    {
        Task<OperationResult<IEnumerable<ZoneDto>>> GetAllZonesAsync();
        Task<OperationResult<ZoneDto>> GetZoneByIdAsync(string zoneId);
        Task<OperationResult<ZoneDto>> UpdateZoneAsync(UpdateZoneDto updateDto);
        Task<OperationResult<bool>> DeleteZoneAsync(string zoneId);
        Task<OperationResult<ZoneDto>> AddZoneAsync(CreateZoneDto createDto);
    }
}
