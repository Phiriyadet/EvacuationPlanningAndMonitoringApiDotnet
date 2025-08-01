using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IZoneRepository : IGenericRepository<Zone, string>
    {
        Task<string?> GetLastIdZoneAsync();
    }
}
