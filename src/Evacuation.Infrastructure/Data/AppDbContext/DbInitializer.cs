using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Evacuation.Infrastructure.Data.AppDbContext;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();

        // Seed สำหรับ Mssql
        var mssql = scope.ServiceProvider.GetService<MssqlDbContext>();
        if (mssql != null)
        {
            await mssql.Database.MigrateAsync();
            await SeedAdminAsync(mssql, config);
        }

        // Seed สำหรับ Postgres
        var pg = scope.ServiceProvider.GetService<PostgresDbContext>();
        if (pg != null)
        {
            await pg.Database.MigrateAsync();
            await SeedAdminAsync(pg, config);
        }
    }

    private static async Task SeedAdminAsync(DbContext context, IConfiguration config)
    {
        if (!await context.Set<User>().AnyAsync(u => u.Role == RoleType.Admin && u.Username == "admin"))
        {
            var adminPassword = config["Admin:Password"]
                ?? throw new InvalidOperationException("Admin password not configured.");

            var adminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

            var admin = new User("admin", "admin@example.com", adminHash, RoleType.Admin);
            context.Set<User>().Add(admin);
            await context.SaveChangesAsync();
        }
    }
}


