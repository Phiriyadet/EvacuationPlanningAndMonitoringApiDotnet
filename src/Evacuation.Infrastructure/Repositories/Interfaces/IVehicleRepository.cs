using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IVehicleRepository : IGenericRepository<Vehicle, string>
    {
        Task<string?> GetLastIdVehicleAsync();
    }
}
