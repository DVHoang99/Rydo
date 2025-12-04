using Microsoft.EntityFrameworkCore;

namespace Rydo.Infrastructure.Persistence
{
    public class RydoDbContext : DbContext
    {
        public RydoDbContext(DbContextOptions<RydoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(RydoDbContext).Assembly);
        }
    }
}