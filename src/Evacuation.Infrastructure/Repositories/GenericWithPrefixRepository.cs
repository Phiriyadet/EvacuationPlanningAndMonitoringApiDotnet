using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class GenericWithPrefixRepository<T, TKey>
            : GenericRepository<T, TKey>, IGenericWithPrefixRepository<T, TKey>
            where T : BaseEntityWithPrefix
            where TKey : notnull
    {
        public GenericWithPrefixRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Dictionary<TKey, string>> GetIdMapAsync()
        {
            return _dbSet.AsNoTracking()
            .ToDictionaryAsync(e => (TKey)(object)e.Id, e => e.BusinessId);
        }
    }
}
