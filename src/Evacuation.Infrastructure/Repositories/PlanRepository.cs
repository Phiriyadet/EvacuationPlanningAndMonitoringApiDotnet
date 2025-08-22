using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class PlanRepository : GenericWithPrefixRepository<Plan, int>, IPlanRepository
    {
        public PlanRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
