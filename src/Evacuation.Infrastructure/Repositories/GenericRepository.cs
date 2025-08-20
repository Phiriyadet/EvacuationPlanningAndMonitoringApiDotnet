using Evacuation.Domain.Entities;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey>
            where T : BaseEntityWithPrefix
            where TKey : notnull
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }

        public async Task<T> AddAsync(T entity)
        {
            var entry = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<bool> DeleteAsync(TKey key)
        {
            var entity = await _dbSet.FindAsync(key);
            if (entity == null)
            {
                return false;
            }
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(TKey key)
        {
            return await _dbSet.FindAsync(key) != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(TKey id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

        public Task<Dictionary<TKey, string>> GetIdMapAsync()
        {
            return _dbSet.AsNoTracking()
            .ToDictionaryAsync(e => (TKey)(object)e.Id, e => e.BusinessId);
        }

        public IQueryable<T> GetQuery()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task<T?> UpdateAsync(TKey key, T entity)
        {
            var existingEntity = await _dbSet.FindAsync(key);
            if (existingEntity == null)
            {
                return null;
            }
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existingEntity;
        }
    }
}
