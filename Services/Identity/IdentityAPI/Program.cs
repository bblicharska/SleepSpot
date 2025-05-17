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


// Early init of NLog to allow startup and exception logging, before host is built
var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5031");

    // Add services to the container.

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Register health checks
    builder.Services.AddHealthChecks();

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen();

    // rejestracja automappera w kontenerze IoC
    builder.Services.AddAutoMapper(typeof(UserMappingProfile));

    //var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    //Console.WriteLine("Program: " + new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])));
    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // .AddJwtBearer(options =>
    // {
    //     options.TokenValidationParameters = new TokenValidationParameters
    //     {
    //         ValidateIssuer = true,
    //         ValidIssuer = jwtSettings["Issuer"],
    //         ValidateAudience = true,
    //         ValidAudience = jwtSettings["Audience"],
    //         ValidateLifetime = true,
    //         ValidateIssuerSigningKey = true,
    //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    //     };
    // });

    //builder.Services.AddAuthorization();

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
    // Map the /health endpoint
    app.MapHealthChecks("/health");

    app.UseMiddleware<ExceptionMiddleware>();

    //app.UseAuthentication();
    //app.UseAuthorization();

    app.MapControllers();

    // wstawia politykê CORS obs³ugi do potoku ¿¹dania
    app.UseCors("SleepSpot");

    // seeding data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<UserDbContext>();
        var seeder = services.GetRequiredService<DataSeeder>();

        // Retry policy to wait if SQL Server isn't ready yet
        var retryPolicy = Policy
            .Handle<SqlException>()
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(5),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"[Startup Retry] Attempt {retryCount} failed. Waiting {timeSpan.TotalSeconds}s. Exception: {exception.Message}");
                });

        retryPolicy.Execute(() =>
        {
            // Ensure database exists (optional)
            //context.Database.EnsureCreated(); // You may remove this if you rely solely on Migrations

            // Apply pending migrations
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

            // Seed initial data
            seeder.Seed();
        });
    }

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
