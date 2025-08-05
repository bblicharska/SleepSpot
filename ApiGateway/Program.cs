using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Add Controllers - IMPORTANT: Add this for your custom controller
builder.Services.AddControllers();

// Add configuration for Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// JWT authentication configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options => // Explicit scheme name
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ClockSkew = TimeSpan.Zero // Reduce clock skew tolerance
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

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);

// HTTP Clients for direct service communication (not through Ocelot)
// Add timeout and retry policies for better resilience
builder.Services.AddHttpClient("PropertyClient", client =>
{
    client.BaseAddress = new Uri("http://property-api:5030");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("ReviewClient", client =>
{
    client.BaseAddress = new Uri("http://review-api:5029");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("UserClient", client =>
{
    client.BaseAddress = new Uri("http://identity-api:5031");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("GroupClient", client =>
{
    client.BaseAddress = new Uri("http://group-api:5032");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add CORS
builder.Services.AddCors(options => options.AddPolicy("SleepSpot", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
}));

// Add Swagger for ApiGateway with JWT configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token with Bearer prefix"
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

var app = builder.Build();

// Configure middleware pipeline - ORDER IS CRITICAL
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SleepSpot");

// Add routing before authentication
app.UseRouting();

// Authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers for custom endpoints (like your GatewayController)
// This must come BEFORE UseOcelot to ensure custom routes take precedence
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
// Configure Ocelot middleware - This handles all remaining routes
await app.UseOcelot();

app.Run();