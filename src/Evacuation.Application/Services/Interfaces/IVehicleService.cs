using Evacuation.Application.DTOs.Vehicle;
using Evacuation.Shared.Result;

namespace Evacuation.Application.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<OperationResult<IEnumerable<VehicleDto>>> GetAllVehiclesAsync();
        Task<OperationResult<VehicleDto>> GetVehicleByIdAsync(int vehicleId);
        Task<OperationResult<VehicleDto>> UpdateVehicleAsync(int Id, UpdateVehicleDto updateDto);
        Task<OperationResult<bool>> DeleteVehicleAsync(int vehicleId);
        Task<OperationResult<VehicleDto>> AddVehicleAsync(CreateVehicleDto createDto);
    }
}
