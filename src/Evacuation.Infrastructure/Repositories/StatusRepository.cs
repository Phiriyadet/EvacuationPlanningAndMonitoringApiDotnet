using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class StatusRepository : GenericWithIncludeRepository<Status, int>, IStatusRepository
    {
        public StatusRepository(IDbContextFactory factory, DatabaseType database) : base(factory, database)
        {
        }
    }
}
