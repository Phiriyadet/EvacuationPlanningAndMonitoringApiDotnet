using Evacuation.Domain.Entities;

namespace Evacuation.Application.Interfaces.IRepositories
{
    public interface IStatusRepository : IGenericRepository<EvacuationStatus, string>
    {
    }
}
