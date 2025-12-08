using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using Rydo.Application;
using Rydo.Application.Common.Helpers;
using Rydo.Application.Common.Interfaces;
using Rydo.Domain.Entities;
using Rydo.Infrastructure.Persistence;
using Rydo.Infrastructure.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
});

// EF Core + Npgsql + NetTopologySuite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    )
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = builder.Configuration["Keycloak:Authority"];
        opts.Audience = "rydo-api";
        opts.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization();

// Geometry factory
builder.Services.AddSingleton(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

// ApplicationDbContext interface
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

builder.Services.AddDataProtection();
builder.Services.AddSingleton<CryptoHelper>();
builder.Services.AddSingleton<IEntityTypeConfiguration<PaymentDetail>, PaymentDetailConfiguration>();

var app = builder.Build();
// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthentication();   // bắt buộc phải trước Authorization
//app.UseAuthorization();
app.MapControllers();

// Minimal API example
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi()
.RequireAuthorization(); // nếu muốn bắt JWT auth

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
