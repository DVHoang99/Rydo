using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rydo.Domain.Entities;

namespace Rydo.Infrastructure.Persistence.Configurations;

public class CarImageConfiguration: IEntityTypeConfiguration<CarImage>
{
    public void Configure(EntityTypeBuilder<CarImage> builder)
    {
        builder.ToTable("CarImages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.IsCover)
            .HasDefaultValue(false);

        builder.HasOne<Car>()
            .WithMany(c => c.Images)
            .HasForeignKey(x => x.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for faster query when loading cover image first
        builder.HasIndex(x => new { x.CarId, x.IsCover });
    }
}