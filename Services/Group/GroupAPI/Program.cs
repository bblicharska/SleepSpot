using AutoMapper;
using GroupAPI.Middleware;
using GroupService.Application.Interfaces;
using GroupService.Application.Mappings;
using GroupService.Application.Services;
using GroupService.Domain.Contracts;
using GroupService.Infrastructure;
using GroupService.Infrastructure.ExternalCients;
using GroupService.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Polly;
using System.Text;
using System.Text.Json.Serialization;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5032");

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

    builder.Services.AddAutoMapper(typeof(GroupMappingProfile));

    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<GroupDbContext>(options =>
      options.UseSqlServer(mssqlConnectionString, sqlOptions => sqlOptions.MigrationsAssembly("GroupService.Infrastructure")));

    builder.Services.AddScoped<IGroupUnitOfWork, GroupUnitOfWork>();
    builder.Services.AddScoped<IGroupRepository, GroupRepository>();
    builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
    builder.Services.AddScoped<IGroupListingRepository, GroupListingRepository>();
    builder.Services.AddScoped<IRoomApplicationRepository, RoomApplicationRepository>();

    builder.Services.AddScoped<DataSeeder>();
    builder.Services.AddScoped<IGroupService, GroupService.Application.Services.GroupService>();
    builder.Services.AddScoped<ExceptionMiddleware>();

    builder.Services.AddHttpClient<IUserClient, UserClient>(client =>
    {
        client.BaseAddress = new Uri("http://identity-api:5031");
    });
    builder.Services.AddHttpClient<IPropertyClient, PropertyClient>(client =>
    {
        client.BaseAddress = new Uri("http://property-api:5030");
    });

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
        var context = services.GetRequiredService<GroupDbContext>();
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

