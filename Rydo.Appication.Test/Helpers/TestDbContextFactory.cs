using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Rydo.Infrastructure.Persistence;
using SQLitePCL;
using Testcontainers.PostgreSql;
using Xunit;

namespace Rydo.Appication.Test.Helpers;

public class TestDbContextFactory : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    public ApplicationDbContext DbContext { get; private set; }

    public TestDbContextFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgis/postgis:16-3.4")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString(), opts =>
            {
                opts.UseNetTopologySuite();  // nếu dùng Point geometry
            })
            .Options;

        DbContext = new ApplicationDbContext(options);

        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}