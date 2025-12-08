using Testcontainers.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Rydo.Infrastructure.Persistence;
using MediatR;
using Rydo.Application;
using Rydo.Application.Common.Interfaces;
using Xunit;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected ServiceProvider _provider = null!;
    private readonly PostgreSqlContainer _postgres;

    protected IntegrationTestBase()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("testdb")
            .WithUsername("test")
            .WithPassword("testpass")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(_postgres.GetConnectionString()));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
        });


        // Register IApplicationDbContext
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        _provider = services.BuildServiceProvider();

        // Auto migrate
        using var scope = _provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }

    public async Task DisposeAsync()
    {
        await _postgres.StopAsync();
    }
}