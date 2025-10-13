using Evacuation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Data.AppDbContext;

public class PostgresDbContext : BaseDbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }
}