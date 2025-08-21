using Evacuation.Application.Services;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Cache;
using Evacuation.Infrastructure.Cache.Interfaces;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

var dbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
if (string.IsNullOrEmpty(dbConnectionString))
{
    throw new ArgumentException("Database connection string is not configured.");
}
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection");
if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new ArgumentException("Redis connection string is not configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(dbConnectionString));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// Add services to the container.
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();

builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IEvacuationService, EvacuationService>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaGeneratorOptions.UseInlineDefinitionsForEnums = true;

    // เพิ่มตรงนี้เพื่อให้ Swagger แสดง enum เป็น string
    options.UseAllOfToExtendReferenceSchemas();
    options.MapType<VehicleType>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(VehicleType))
                   .Select(name => new OpenApiString(name))
                   .Cast<IOpenApiAny>()
                   .ToList()
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
