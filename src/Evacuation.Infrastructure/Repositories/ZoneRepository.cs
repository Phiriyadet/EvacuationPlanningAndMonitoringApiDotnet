using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class ZoneRepository : GenericWithIncludeRepository<Zone, int>, IZoneRepository
    {
        public ZoneRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
