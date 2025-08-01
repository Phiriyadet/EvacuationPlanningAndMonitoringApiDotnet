using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle, string>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<string?> GetLastIdVehicleAsync()
        {
            return await _dbSet
                .OrderByDescending(v => v.VehicleId)
                .Select(v => v.VehicleId)
                .FirstOrDefaultAsync();
        }

    }
}
