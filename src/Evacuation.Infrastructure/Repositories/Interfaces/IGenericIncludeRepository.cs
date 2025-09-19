using System.Linq.Expressions;
using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IGenericIncludeRepository<T, TKey> : IGenericRepository<T, TKey>
            where T : BaseEntityWithPrefix
            where TKey : notnull
    {
        Task<Dictionary<TKey, string>> GetIdMapAsync();
        Task<IEnumerable<T>> GetAllWithInclude(params Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdWithInclude(TKey key, params Expression<Func<T, object>>[] includes);
    }
}
