using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Polly;
using PropertyAPI.Middleware;
using PropertyService.Application.Mappings;
using PropertyService.Application.Services;
using PropertyService.Domain.Contracts;
using PropertyService.Infrastructure;
using PropertyService.Infrastructure.Repositories;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var options = new WebApplicationOptions
    {
        WebRootPath = "wwwroot"
    };
    var builder = WebApplication.CreateBuilder(options);

    builder.WebHost.UseUrls("http://0.0.0.0:5030");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddHealthChecks();

    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

    builder.Services.AddEndpointsApiExplorer();

    builder.Configuration.AddEnvironmentVariables();

    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(PropertyMappingProfile));

    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<PropertyDbContext>(options =>
      options.UseSqlServer(mssqlConnectionString, sqlOptions => sqlOptions.MigrationsAssembly("PropertyService.Infrastructure")));

    builder.Services.AddScoped<IPropertyUnitOfWork, PropertyUnitOfWork>();
    builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
    builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
    builder.Services.AddScoped<IRoomRepository, RoomRepository>();

    builder.Services.AddScoped<DataSeeder>();
    builder.Services.AddScoped<IPropertyService, PropertyManager>();
    builder.Services.AddScoped<ExceptionMiddleware>();

    builder.Services.AddCors(o => o.AddPolicy("SleepSpot", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    }));

    builder.Logging.AddConsole();
    var app = builder.Build();

    var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads", "properties");
    if (!Directory.Exists(uploadsPath))
    {
        Directory.CreateDirectory(uploadsPath);
        Console.WriteLine($"Created uploads directory at: {uploadsPath}");
    }
    Console.WriteLine($"Static File Path: {uploadsPath}");

    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/uploads"))
        {
            Console.WriteLine($"Static file request: {context.Request.Method} {context.Request.Path}");
            Console.WriteLine($"Physical path would be: {Path.Combine(app.Environment.WebRootPath, context.Request.Path.Value.TrimStart('/'))}");
        }
        await next();
    });

    app.UseCors("SleepSpot");

    app.UseStaticFiles();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.WebRootPath, "uploads")),
        RequestPath = "/uploads",
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Methods", "GET");
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=3600");

            Console.WriteLine($"Serving static file: {ctx.File.Name} from {ctx.File.PhysicalPath}");
        }
    });

    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Unhandled exception");
            throw;
        }
    });

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapHealthChecks("/health");

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseRouting();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PropertyDbContext>();
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