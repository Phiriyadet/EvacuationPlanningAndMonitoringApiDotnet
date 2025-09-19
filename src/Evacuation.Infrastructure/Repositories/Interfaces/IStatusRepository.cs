using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IStatusRepository : IGenericIncludeRepository<Status, int>
    {
    }
}
