using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class StatusRepository : GenericWithPrefixRepository<Status, int>, IStatusRepository
    {
        public StatusRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
