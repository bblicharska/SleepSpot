using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Polly;
using ReviewAPI.Middleware;
using ReviewService.Application.Mappings;
using ReviewService.Application.Services;
using ReviewService.Domain.Contracts;
using ReviewService.Infrastructure;
using ReviewService.Infrastructure.Repositories;
using System.Text;
using ReviewService.Application.Services;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5029");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHealthChecks();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(ReviewMappingProfile));

    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<ReviewDbContext>(options =>
      options.UseSqlServer(mssqlConnectionString, sqlOptions => sqlOptions.MigrationsAssembly("ReviewService.Infrastructure")));

    builder.Services.AddScoped<IReviewUnitOfWork, ReviewUnitOfWork>();
    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

    builder.Services.AddScoped<DataSeeder>();
    builder.Services.AddScoped<IReviewService, ReviewService.Application.Services.ReviewService>();
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
        var context = services.GetRequiredService<ReviewDbContext>();
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

