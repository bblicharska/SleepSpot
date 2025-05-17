using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5030");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Register health checks
    builder.Services.AddHealthChecks();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Configuration.AddEnvironmentVariables();

    //builder.Services.AddSwaggerGen(options =>
    //{
    //    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //    {
    //        In = ParameterLocation.Header,
    //        Name = "Authorization",
    //        Type = SecuritySchemeType.ApiKey,
    //        BearerFormat = "JWT",
    //        Description = "Enter 'Bearer' followed by a space and your JWT token"
    //    });

    //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //    {
    //        {
    //            new OpenApiSecurityScheme
    //            {
    //                Reference = new OpenApiReference
    //                {
    //                    Type = ReferenceType.SecurityScheme,
    //                    Id = "Bearer"
    //                }
    //            },
    //            new string[] {}
    //        }
    //    });
    //});
    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(PropertyMappingProfile));

    //var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //    .AddJwtBearer(options =>
    //    {
    //        options.TokenValidationParameters = new TokenValidationParameters
    //        {
    //            ValidateIssuer = true,
    //            ValidIssuer = jwtSettings["Issuer"],
    //            ValidateAudience = true,
    //            ValidAudience = jwtSettings["Audience"],
    //            ValidateLifetime = true,
    //            ValidateIssuerSigningKey = true,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    //        };
    //    });

    //builder.Services.AddAuthorization();


    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    //builder.Services.AddDbContext<PropertyDbContext>(options =>
    // options.UseSqlServer(mssqlConnectionString));
    builder.Services.AddDbContext<PropertyDbContext>(options =>
      options.UseSqlServer(mssqlConnectionString, sqlOptions => sqlOptions.MigrationsAssembly("PropertyService.Infrastructure")));


    //builder.Services.AddHttpClient();

    builder.Services.AddScoped<IPropertyUnitOfWork, PropertyUnitOfWork>();
    builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
    builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();

    builder.Services.AddScoped<DataSeeder>();
    builder.Services.AddScoped<IPropertyService, PropertyManager>();
    builder.Services.AddScoped<ExceptionMiddleware>();

    builder.Services.AddCors(o => o.AddPolicy("SleepSpot", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    }));


    var app = builder.Build();

    app.UseStaticFiles();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Map the /health endpoint
    app.MapHealthChecks("/health");

    app.UseMiddleware<ExceptionMiddleware>();

    //app.UseAuthentication();
    //app.UseAuthorization();

    app.MapControllers();

    app.UseCors("SleepSpot");

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

