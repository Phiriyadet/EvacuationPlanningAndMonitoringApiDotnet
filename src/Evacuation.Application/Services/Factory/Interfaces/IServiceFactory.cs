using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;

namespace Evacuation.Application.Services.Factory.Interfaces;

public interface IServiceFactory
{
    IVehicleService CreateVehicleService(DatabaseType dbType);
    IZoneService CreateZoneService(DatabaseType dbType);
    IEvacuationService CreateEvacuationService(DatabaseType dbType);
    IUserService CreateUserService(DatabaseType dbType);
}
