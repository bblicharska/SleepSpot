using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Dodaj konfiguracjê Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Dodaj us³ugi Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Dodaj CORS
builder.Services.AddCors(o => o.AddPolicy("SleepSpot", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

// Dodaj Swagger dla ApiGateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("SleepSpot");

// Skonfiguruj Ocelot middleware
await app.UseOcelot();

app.Run();