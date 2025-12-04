using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rydo.Domain.Entities;

namespace Rydo.Infrastructure.Persistence.Configurations;

public class CarConfiguration: IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Brand).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Model).HasMaxLength(50).IsRequired();

        builder.Property(x => x.Address).HasMaxLength(200);

        builder.Property(x => x.Location)
            .HasColumnType("geometry(Point,4326)");

        builder.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(i => i.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}