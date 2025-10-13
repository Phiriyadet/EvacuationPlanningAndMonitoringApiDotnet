using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Repositories.Factory.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;

namespace Evacuation.Infrastructure.Repositories.Factory;

public class RepositoryFactory : IRepositoryFactory
{
    private readonly IDbContextFactory _dbContextFactory;
    public RepositoryFactory(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IPlanRepository CreatePlanRepository(DatabaseType dbType)
    {
        return new PlanRepository(_dbContextFactory, dbType);
    }

    public IStatusRepository CreateStatusRepository(DatabaseType dbType)
    {
        return new StatusRepository(_dbContextFactory, dbType);
    }

    public IUserRepository CreateUserRepository(DatabaseType dbType)
    {
        return new UserRepository(_dbContextFactory, dbType);
    }

    public IVehicleRepository CreateVehicleRepository(DatabaseType dbType)
    {
        return new VehicleRepository(_dbContextFactory, dbType);
    }

    public IZoneRepository CreateZoneRepository(DatabaseType dbType)
    {
        return new ZoneRepository(_dbContextFactory, dbType);
    }
}
