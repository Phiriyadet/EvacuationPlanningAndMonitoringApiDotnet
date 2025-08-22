using Evacuation.Domain.Entities;

namespace Evacuation.Infrastructure.Repositories.Interfaces
{
    public interface IGenericRepository<T, TKey>
        where T : BaseEntity
        where TKey : notnull
    {
        Task<T?> GetByIdAsync(TKey id);
        IQueryable<T> GetQuery();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T?> UpdateAsync(TKey key, T entity);
        Task<bool> DeleteAsync(TKey key);
        Task<bool> ExistsAsync(TKey key);
    }
 
}
