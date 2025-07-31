using Evacuation.Domain.Entities;

namespace Evacuation.Application.Interfaces.IRepositories
{
    public interface IZoneRepository : IGenericRepository<Zone, string>
    {
        Task<string?> GetLastIdZoneAsync();
    }
}
