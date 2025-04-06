using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using PropertyAPI.Middleware;
using PropertyService.Application.Mappings;
using PropertyService.Application.Services;
using PropertyService.Domain.Contracts;
using PropertyService.Infrastructure;
using PropertyService.Infrastructure.Repositories;


var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls("http://0.0.0.0:5030");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

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
    //            ValidIssuer = jwtSettings["Issuer"], // Issuer z IdentityService
    //            ValidateAudience = true,
    //            ValidAudience = jwtSettings["Audience"], // Audience z IdentityService
    //            ValidateLifetime = true, // Sprawdza czas wa¿noœci tokenu
    //            ValidateIssuerSigningKey = true, // W³¹czenie weryfikacji klucza podpisuj¹cego
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    //        };
    //    });

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

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.UseCors("SleepSpot");

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PropertyDbContext>();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        dataSeeder.Seed();
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

