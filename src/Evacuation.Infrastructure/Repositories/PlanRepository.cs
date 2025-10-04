using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class PlanRepository : GenericWithIncludeRepository<Plan, int>, IPlanRepository
    {
        public PlanRepository(IDbContextFactory factory, DatabaseType database) : base(factory, database)
        {
        }
    }
}
