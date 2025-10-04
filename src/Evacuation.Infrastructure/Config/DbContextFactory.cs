using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Data.AppDbContext;
using Microsoft.EntityFrameworkCore;

namespace Evacuation.Infrastructure.Config;

public class DbContextFactory : IDbContextFactory
{
    private readonly MssqlDbContext _mssql;
    private readonly PostgresDbContext _postgres;
    public DbContextFactory(MssqlDbContext mssql, PostgresDbContext postgres)
    {
        _mssql = mssql;
        _postgres = postgres;
    }
    public DbContext GetDbContext(DatabaseType dbType)
    {
        switch (dbType)
        {
            case DatabaseType.Mssql:
                return _mssql;
            case DatabaseType.Postgres:
                return _postgres;
            default:
                throw new ArgumentException("Unsupported DB type.");
        }
    }
}
