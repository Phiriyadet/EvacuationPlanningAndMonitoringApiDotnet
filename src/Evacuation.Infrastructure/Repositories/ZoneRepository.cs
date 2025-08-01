using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class ZoneRepository : GenericRepository<Zone, string>, IZoneRepository
    {
        public ZoneRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<string?> GetLastIdZoneAsync()
        {
            return _dbSet
                .OrderByDescending(z => z.ZoneId)
                .Select(z => z.ZoneId)
                .FirstOrDefaultAsync();
        }
    }
}
