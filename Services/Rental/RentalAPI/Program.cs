using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Polly;
using RentalAPI.Middleware;
using RentalService.Application.Interfaces;
using RentalService.Application.Mappings;
using RentalService.Application.Services;
using RentalService.Domain.Contracts;
using RentalService.Infrastructure;
using RentalService.Infrastructure.ExternalClients;
using RentalService.Infrastructure.Repositories;
using RentalService.Infrastructure.Workers;
using System.Text;
using System.Text.Json.Serialization;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5033");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHealthChecks();

    builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    builder.Services.AddEndpointsApiExplorer();

    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(RentalAgreementMappingProfile));

    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<RentalDbContext>(options =>
      options.UseSqlServer(mssqlConnectionString, sqlOptions => sqlOptions.MigrationsAssembly("RentalService.Infrastructure")));

    builder.Services.AddScoped<IRentalUnitOfWork, RentalUnitOfWork>();
    builder.Services.AddScoped<IRentalAgreementRepository, RentalAgreementRepository>();

    builder.Services.AddScoped<DataSeeder>();
    builder.Services.AddScoped<IRentalService, RentalService.Application.Services.RentalService>();
    builder.Services.AddScoped<ExceptionMiddleware>();

    builder.Services.AddHttpClient<IUserClient, UserClient>(client =>
    {
        client.BaseAddress = new Uri("http://identity-api:5031");
    });
    builder.Services.AddHttpClient<IGroupClient, GroupClient>(client =>
    {
        client.BaseAddress = new Uri("http://group-api:5032");
    });
    builder.Services.AddHttpClient<IPropertyClient, PropertyClient>(client =>
    {
        client.BaseAddress = new Uri("http://property-api:5030");
    });

    builder.Services.AddCors(o => o.AddPolicy("SleepSpot", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));
    builder.Services.AddHostedService<ExpiredRentalWorker>();

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
        var context = services.GetRequiredService<RentalDbContext>();
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

