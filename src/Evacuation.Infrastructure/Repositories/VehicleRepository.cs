using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories
{
    public class VehicleRepository : GenericWithIncludeRepository<Vehicle, int>, IVehicleRepository
    {
        public VehicleRepository(IDbContextFactory factory, DatabaseType database) : base(factory, database)
        {
        }
    }
}
