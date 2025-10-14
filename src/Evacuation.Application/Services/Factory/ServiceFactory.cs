using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Evacuation.Application.Services.Factory.Interfaces;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Cache.Interfaces;
using Evacuation.Infrastructure.Repositories.Factory.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Evacuation.Application.Services.Factory;

public class ServiceFactory : IServiceFactory
{
    private readonly IRepositoryFactory _repoFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private readonly ICacheService _cacheService;

    public ServiceFactory(IRepositoryFactory repoFactory, IServiceProvider serviceProvider, IConfiguration config, ICacheService cacheService)
    {
        _repoFactory = repoFactory;
        _serviceProvider = serviceProvider;
        _config = config;
        _cacheService = cacheService;
    }

    public IEvacuationService CreateEvacuationService(DatabaseType dbType)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<EvacuationService>>();
        return new EvacuationService(_repoFactory, dbType, _cacheService, logger);
    }

    public IUserService CreateUserService(DatabaseType dbType)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<UserService>>();
        return new UserService(_config, _repoFactory, dbType, logger);
    }

    public IVehicleService CreateVehicleService(DatabaseType dbType)
    {
        // resolve logger จาก DI container
        var logger = _serviceProvider.GetRequiredService<ILogger<VehicleService>>();
        return new VehicleService(_repoFactory, dbType, logger);
    }

    public IZoneService CreateZoneService(DatabaseType dbType)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<ZoneService>>();
        return new ZoneService(_repoFactory, dbType, logger);
    }
}

