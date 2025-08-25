using Evacuation.Domain.Entities;
using Evacuation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Evacuation.Infrastructure.Data.AppDbContext
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations (optional, ถ้าอยากให้ auto migrate ตอน start)
            await context.Database.MigrateAsync();

            // --- Seed Admin User ---
            if (!context.Users.Any(u => u.Role == RoleType.Admin && u.Username == "admin"))
            {
                var adminPassword = config["Admin:Password"];
                if (string.IsNullOrEmpty(adminPassword))
                    throw new InvalidOperationException("Admin password not configured.");

                var adminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);

                var adminUser = new User(
                    username: "admin",
                    email: "admin@example.com",
                    passwordHash: adminHash,
                    role: RoleType.Admin
                );

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }

}
