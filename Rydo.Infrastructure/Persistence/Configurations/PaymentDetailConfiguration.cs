using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rydo.Application.Common.Enums;
using Rydo.Application.Common.Helpers;
using Rydo.Domain.Entities;

namespace Rydo.Infrastructure.Persistence.Configurations;

public class PaymentDetailConfiguration(CryptoHelper crypto) : IEntityTypeConfiguration<PaymentDetail>
{
    public void Configure(EntityTypeBuilder<PaymentDetail> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Created)
            .HasDefaultValueSql("NOW()");
        builder.Property(x => x.Created)
            .HasDefaultValueSql("NOW()");
        builder.Property(x => x.BookingId).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Status).HasDefaultValue(PaymentStatus.Pending);

        builder.Property(x => x.Detail)
            .HasConversion(new EncryptedDetailConverter(crypto))
            .HasColumnType("text");
    }
}