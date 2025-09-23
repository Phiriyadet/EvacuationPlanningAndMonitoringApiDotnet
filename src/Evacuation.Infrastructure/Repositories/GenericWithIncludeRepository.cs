using System.Linq.Expressions;
using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class GenericWithIncludeRepository<T, TKey>
            : GenericRepository<T, TKey>, IGenericIncludeRepository<T, TKey>
            where T : BaseEntityWithPrefix
            where TKey : notnull
    {
        public GenericWithIncludeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = GetQuery();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdWithIncludeAsync(TKey key, params Expression<Func<T, object>>[] includes)
        {
            var query = GetQuery();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(q => q.Id.Equals(key));
        }
    }
}
