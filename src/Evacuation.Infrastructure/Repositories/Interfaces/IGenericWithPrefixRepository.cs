using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IGenericWithPrefixRepository<T, TKey> : IGenericRepository<T, TKey>
            where T : BaseEntityWithPrefix
            where TKey : notnull
    {
        Task<Dictionary<TKey, string>> GetIdMapAsync();
    }
}
