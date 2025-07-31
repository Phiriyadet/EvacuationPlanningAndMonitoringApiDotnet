using Evacuation.Domain.Entities;

namespace Evacuation.Application.Interfaces.IRepositories
{
    public interface IVehicleRepository : IGenericRepository<Vehicle, string>
    {
        Task<string?> GetLastIdVehicleAsync();
    }
}
