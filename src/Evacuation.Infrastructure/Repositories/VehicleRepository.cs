using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class VehicleRepository : GenericWithPrefixRepository<Vehicle, int>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
