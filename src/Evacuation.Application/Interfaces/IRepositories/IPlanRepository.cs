using Evacuation.Domain.Entities;

namespace Evacuation.Application.Interfaces.IRepositories
{
    public interface IPlanRepository : IGenericRepository<Plan, string>
    {
        Task<string?> GetLastIdPlanAsync();
    }
}
