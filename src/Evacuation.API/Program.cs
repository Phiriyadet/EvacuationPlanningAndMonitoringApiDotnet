using Evacuation.Application.Services;
using Evacuation.Application.Services.Interfaces;
using Evacuation.Domain.Enums;
using Evacuation.Infrastructure.Cache;
using Evacuation.Infrastructure.Cache.Interfaces;
using Evacuation.Infrastructure.Data.AppDbContext;
using Evacuation.Infrastructure.Repositories;
using Evacuation.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

// Configure Serilog
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // แยกไฟล์รายวัน
//    .CreateLogger();

//builder.Host.UseSerilog(); // ใช้ Serilog แทน default logger

#region ConnectionString
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
#endregion

#region Servives Register
// Add services to the container.
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IEvacuationService, EvacuationService>();
builder.Services.AddScoped<IUserService, UserService>();
#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// กำหนดค่า Swagger สำหรับ API
/// </summary>
builder.Services.AddSwaggerGen(c =>
{
    /// <summary>
    /// กำหนดข้อมูลเอกสาร API เช่น Title, Version
    /// </summary>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    /// <summary>
    /// เพิ่ม Security Definition เพื่อให้ Swagger รองรับ JWT Authentication
    /// - In = Header → JWT จะถูกส่งใน Header
    /// - Name = "Authorization" → ใช้ชื่อ header ว่า Authorization
    /// - Type = ApiKey → ประเภทเป็น API Key แต่ใช้รูปแบบ Bearer Token
    /// - Scheme = Bearer → กำหนดว่าใช้ Bearer token
    /// </summary>
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    /// <summary>
    /// กำหนดว่า ทุกๆ Request ใน Swagger จะต้องมี JWT Authorization ติดไปด้วย
    /// </summary>
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // ใช้ค่า Bearer ที่เราสร้างด้านบน
                }
            },
            Array.Empty<string>() // ไม่มี Scope เพิ่มเติม
        }
    });
});


/// <summary>
/// กำหนดการทำงานของ Authentication โดยใช้ JWT
/// </summary>
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        /// <summary>
        /// ตั้งค่าเงื่อนไขการตรวจสอบความถูกต้องของ JWT
        /// </summary>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            /// <summary>
            /// ตรวจสอบว่า Issuer (ผู้ออก Token) ถูกต้องหรือไม่
            /// </summary>
            ValidateIssuer = true,

            /// <summary>
            /// ตรวจสอบว่า Audience (กลุ่มผู้ใช้งาน Token) ถูกต้องหรือไม่
            /// </summary>
            ValidateAudience = true,

            /// <summary>
            /// ตรวจสอบว่า Token หมดอายุหรือยัง
            /// </summary>
            ValidateLifetime = true,

            /// <summary>
            /// ตรวจสอบว่า Key สำหรับเข้ารหัส Token ถูกต้องหรือไม่
            /// </summary>
            ValidateIssuerSigningKey = true,

            /// <summary>
            /// ค่า Issuer ที่เรากำหนดไว้ใน appsettings.json (Jwt:Issuer)
            /// </summary>
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            /// <summary>
            /// ค่า Audience ที่เรากำหนดไว้ใน appsettings.json (Jwt:Audience)
            /// </summary>
            ValidAudience = builder.Configuration["Jwt:Audience"],

            /// <summary>
            /// Key ลับ (Secret Key) ที่ใช้เข้ารหัส Token
            /// ต้องตรงกับตอนสร้าง Token ไม่งั้นจะ validate ไม่ผ่าน
            /// </summary>
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();
    await DbInitializer.SeedAsync(services, config);
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
