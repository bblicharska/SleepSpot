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

    // Register health checks
    builder.Services.AddHealthChecks();

    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references
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
        builder.WithOrigins("http://localhost:3000") // Only allow your React app
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Optional: Needed if using cookies/auth
    }));

    builder.Logging.AddConsole();
    var app = builder.Build();

    // Ensure the uploads directory exists
    var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads", "properties");
    if (!Directory.Exists(uploadsPath))
    {
        Directory.CreateDirectory(uploadsPath);
        Console.WriteLine($"Created uploads directory at: {uploadsPath}");
    }
    Console.WriteLine($"Static File Path: {uploadsPath}");

    // Add logging middleware to debug requests (before static files)
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/uploads"))
        {
            Console.WriteLine($"Static file request: {context.Request.Method} {context.Request.Path}");
            Console.WriteLine($"Physical path would be: {Path.Combine(app.Environment.WebRootPath, context.Request.Path.Value.TrimStart('/'))}");
        }
        await next();
    });

    // Configure CORS before static files
    app.UseCors("SleepSpot");

    // Default static files middleware
    app.UseStaticFiles();

    // Configure static files for uploads with enhanced logging and CORS
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.WebRootPath, "uploads")),
        RequestPath = "/uploads",
        OnPrepareResponse = ctx =>
        {
            // Add CORS headers for images
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Methods", "GET");
            ctx.Context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=3600");

            Console.WriteLine($"Serving static file: {ctx.File.Name} from {ctx.File.PhysicalPath}");
        }
    });

    // Global exception handling
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

    // Map the /health endpoint
    app.MapHealthChecks("/health");

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseRouting();

    app.MapControllers();

    // Database initialization
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<PropertyDbContext>();
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

    Console.WriteLine("PropertyAPI is ready and listening on http://0.0.0.0:5030");
    Console.WriteLine($"Static files will be served from: {uploadsPath}");
    Console.WriteLine("Available endpoints:");
    Console.WriteLine("  - /health (health check)");
    Console.WriteLine("  - /uploads/* (static files)");
    Console.WriteLine("  - /swagger (API documentation)");

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