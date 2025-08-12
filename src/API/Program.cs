using Core.Services;
using Core.Services.Implementations;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Reflection;
using SendGrid;
using SendGrid.Helpers.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Disaster Alert System API",
        Version = "v1",
        Description = "A comprehensive API for managing disaster alerts and regional notifications",
        Contact = new OpenApiContact
        {
            Name = "Disaster Alert System Team",
            Email = "support@disasteralertsystem.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Add XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("API")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

// Register services
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IDisasterTypeRepository, DisasterTypeRepository>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<IAlertSettingService, AlertSettingService>();
builder.Services.AddScoped<IDisasterRisksService, DisasterRisksService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IEmailService, SendGridEmailService>();

// Register repositories
builder.Services.AddScoped<IAlertSettingRepository, AlertSettingRepository>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();



// Register SendGrid client
builder.Services.AddSingleton<ISendGridClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["ExternalApis:SendGrid:ApiKey"];
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("SendGrid API key is not configured. Please check your configuration.");
    }
    return new SendGridClient(apiKey);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for development and testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Disaster Alert System API V1");
    c.RoutePrefix = "swagger"; // This makes it available at /swagger/index.html
    c.DocumentTitle = "Disaster Alert System API Documentation";
    c.DefaultModelsExpandDepth(2);
    c.DefaultModelExpandDepth(3);
    c.DisplayRequestDuration();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseHttpsRedirection();

// Add global exception handler middleware
app.UseGlobalExceptionHandler();

app.UseAuthorization();
app.MapControllers();

app.Run();
