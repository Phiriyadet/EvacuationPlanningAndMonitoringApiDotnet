using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Evacuation.Application.Services.Factory;
using Evacuation.Application.Services.Factory.Interfaces;
using Evacuation.Infrastructure.Cache;
using Evacuation.Infrastructure.Cache.Interfaces;
using Evacuation.Infrastructure.Config;
using Evacuation.Infrastructure.Config.Interfaces;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories;
using Evacuation.Infrastructure.Repositories.Factory;
using Evacuation.Infrastructure.Repositories.Factory.Interfaces;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;

namespace Evacuation.API;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region 🧩 Configuration Setup
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables();
        #endregion

        #region 🪵 Serilog Configuration
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // log แยกตามวัน
            .CreateLogger();
        builder.Host.UseSerilog();
        #endregion

        #region 🧠 Database & Redis Connection
        // ✅ MSSQL
        var mssql = builder.Configuration.GetConnectionString("MssqlConnection")
            ?? throw new ArgumentException("Mssql connection string not found.");
        builder.Services.AddDbContext<MssqlDbContext>(opt => opt.UseSqlServer(mssql));

        // ✅ Postgres
        var postgres = builder.Configuration.GetConnectionString("PostgresConnection")
            ?? throw new ArgumentException("Postgres connection string not found.");
        builder.Services.AddDbContext<PostgresDbContext>(opt => opt.UseNpgsql(postgres));

        // ✅ Redis
        var redis = builder.Configuration.GetConnectionString("RedisConnection")
            ?? throw new ArgumentException("Redis connection string not found.");
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redis));
        #endregion

        #region ⚙️ Dependency Injection
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        builder.Services.AddScoped(typeof(IGenericIncludeRepository<,>), typeof(GenericWithIncludeRepository<,>));

        builder.Services.AddScoped<IDbContextFactory, DbContextFactory>();
        builder.Services.AddScoped<IServiceFactory, ServiceFactory>();
        builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        #endregion

        #region 📦 Controller & JSON Options
        builder.Services.AddControllers()
            .AddJsonOptions(opt =>
            {
                // แปลง Enum เป็น string (เช่น "Pending" แทน 0)
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        #endregion

        #region 🔐 JWT Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };
            });
        #endregion

        #region 🚦 Rate Limiting
        builder.Services.AddRateLimiter(opt =>
        {
            // Global policy (ทุก request ทั่วไป)
            opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                RateLimitPartition.GetFixedWindowLimiter("global", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 20, // จำกัด 20 req / 10 วินาที
                    Window = TimeSpan.FromSeconds(10),
                    QueueLimit = 2
                }));

            // Policy พิเศษ (เช่น endpoint ที่สำคัญ)
            opt.AddFixedWindowLimiter("strict", o =>
            {
                o.PermitLimit = 5;
                o.Window = TimeSpan.FromSeconds(10);
                o.QueueLimit = 1;
            });
        });
        #endregion

        #region 🧭 API Versioning
        builder.Services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = new UrlSegmentApiVersionReader(); // /api/v1/...
        });

        builder.Services.AddVersionedApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'VVV"; // เช่น v1, v2
            opt.SubstituteApiVersionInUrl = true;
        });
        #endregion

        #region 📘 Swagger Configuration
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // เอกสาร Swagger หลัก
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Evacuation Planning & Monitoring API",
                Version = "v1",
                Description = "API for evacuation planning, vehicle assignment, and monitoring"
            });

            // 🔐 เพิ่ม JWT Authentication ใน Swagger UI
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                Description = "ใส่ JWT Token รูปแบบ: Bearer {token}"
            });

            // 🔐 ให้ Swagger ทุก endpoint รองรับ Bearer Token
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        #endregion

        var app = builder.Build();

        #region 🌱 Database Seeding (Initial Data)
        // ✅ Seed ข้อมูลเริ่มต้นเมื่อแอปเริ่มทำงาน
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var config = services.GetRequiredService<IConfiguration>();

            try
            {
                await DbInitializer.SeedAsync(services, config);
                Log.Information("✅ Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ An error occurred while seeding the database.");
            }
        }
        #endregion

        #region 🌐 Middleware Pipeline
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var groupName in provider.ApiVersionDescriptions.Select(desc => desc.GroupName))
            {
                options.SwaggerEndpoint(
                    $"/swagger/{groupName}/swagger.json",
                    groupName.ToUpperInvariant());
            }
        });


        app.UseHttpsRedirection();
        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        #endregion

        await app.RunAsync();
    }
}
