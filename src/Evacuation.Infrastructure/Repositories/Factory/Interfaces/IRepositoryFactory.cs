using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories.Factory.Interfaces;

public interface IRepositoryFactory
{
    IVehicleRepository CreateVehicleRepository(DatabaseType dbType);
    IZoneRepository CreateZoneRepository(DatabaseType dbType);
    IPlanRepository CreatePlanRepository(DatabaseType dbType);
    IStatusRepository CreateStatusRepository(DatabaseType dbType);
    IUserRepository CreateUserRepository(DatabaseType dbType);
}
