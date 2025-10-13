using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Data.AppDbContext;

public class MssqlDbContext : BaseDbContext
{
    public MssqlDbContext(DbContextOptions<MssqlDbContext> options) : base(options)
    {
    }  
}
