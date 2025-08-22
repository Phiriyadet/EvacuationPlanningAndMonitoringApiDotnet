using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IVehicleRepository : IGenericWithPrefixRepository<Vehicle, int>
    {
    }
}
