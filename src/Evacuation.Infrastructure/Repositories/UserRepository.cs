using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        public UserRepository(IDbContextFactory factory, DatabaseType database) : base(factory, database)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);   
        }
    }
}