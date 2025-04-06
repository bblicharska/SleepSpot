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

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Please enter token"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    });

    // rejestracja automappera w kontenerze IoC
    builder.Services.AddAutoMapper(typeof(UserMappingProfile));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    Console.WriteLine("Program: " + new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])));
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidIssuer = jwtSettings["Issuer"],
              ValidateAudience = true,
              ValidAudience = jwtSettings["Audience"],
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
          };

          options.Events = new JwtBearerEvents
          {
              OnAuthenticationFailed = context =>
              {
                  Console.WriteLine("Authentication failed: " + context.Exception.Message);
                  return Task.CompletedTask;
              },
              OnTokenValidated = context =>
              {
                  Console.WriteLine("Token validated successfully!");
                  return Task.CompletedTask;
              }
          };
      });

    var mssqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.AddSingleton(sp =>
        sp.GetRequiredService<IOptions<JwtSettings>>().Value);
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
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        dataSeeder.Seed();  // Seeding danych (jeœli potrzebujesz)
    }

    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // wstawia politykê CORS obs³ugi do potoku ¿¹dania
    app.UseCors("SleepSpot");

    // seeding data

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
