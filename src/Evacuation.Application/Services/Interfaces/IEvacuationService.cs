using Evacuation.Application.DTOs.Plan;
using Evacuation.Application.DTOs.Status;
using Evacuation.Shared.Result;

namespace Evacuation.Application.Services.Interfaces
{
    public interface IEvacuationService
    {
        Task<OperationResult<IEnumerable<StatusDto>>> GetAllStatusAsync();
        Task<OperationResult<StatusDto>> GetStatusByIdAsync(string statusId);
        Task<OperationResult<IEnumerable<StatusDto>>> UpdateStatusByPlanAsync();
        Task<OperationResult<IEnumerable<PlanDto>>> CreatePlanAsync(double distanceKm);
        Task<OperationResult<IEnumerable<PlanDto>>> GetPlanByZoneIdAsync(string zoneId);
        Task<OperationResult<IEnumerable<PlanDto>>> GetAllPlansAsync();
        Task<OperationResult<bool>> ClearAllPlanAndStatusAsync();


    }
}
