using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using IdentityAPI.Middleware;
using IdentityService.Application.Configurations;
using IdentityService.Application.Mappings;
using IdentityService.Application.Services;
using IdentityService.Domain.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityService.Infrastructure;
using NLog.Web;
using IdentityService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using NLog;
using Microsoft.Data.SqlClient;
using Polly;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.SqlServer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5031");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHealthChecks();

    builder.Services.AddControllers();
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(UserMappingProfile));

    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.AddSingleton(sp =>
        sp.GetRequiredService<IOptions<JwtSettings>>().Value);
    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseSqlServer(mssqlConnectionString, sqlOptions =>
            sqlOptions.MigrationsAssembly("IdentityService.Infrastructure")));

    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserUnitOfWork, UserUnitOfWork>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

    builder.Services.AddScoped<DataSeeder>();

    builder.Services.AddScoped<ExceptionMiddleware>();

    builder.Services.AddCors(o => o.AddPolicy("SleepSpot", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));


    var app = builder.Build();

    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapHealthChecks("/health");

    app.UseMiddleware<ExceptionMiddleware>();

    app.MapControllers();

    app.UseCors("SleepSpot");

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<UserDbContext>();
        var seeder = services.GetRequiredService<DataSeeder>();

        var retryPolicy = Policy
            .Handle<SqlException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"[Startup Retry] Attempt {retryCount} failed. Waiting {timeSpan.TotalSeconds}s. Exception: {exception.Message}");
                });

        retryPolicy.Execute(() =>
        {
            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("Applying pending migrations...");
                context.Database.Migrate();
            }
            else
            {
                Console.WriteLine("No pending migrations.");
            }

            seeder.Seed();
        });
    }

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
