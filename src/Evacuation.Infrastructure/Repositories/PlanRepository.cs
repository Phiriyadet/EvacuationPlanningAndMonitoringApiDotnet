using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class PlanRepository : GenericRepository<Plan, string>, IPlanRepository
    {
        public PlanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<string?> GetLastIdPlanAsync()
        {
            return await _dbSet
                .OrderByDescending(p => p.PlanId)
                .Select(p => p.PlanId)
                .FirstOrDefaultAsync();
        }
    }
}
