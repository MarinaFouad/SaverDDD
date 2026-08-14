namespace WealthOS.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Entities;

public class NetWorthSnapshotConfiguration : IEntityTypeConfiguration<NetWorthSnapshot>
{
    public void Configure(EntityTypeBuilder<NetWorthSnapshot> builder)
    {
        builder.ToTable("NetWorthSnapshots");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.Date).IsRequired();

        builder.OwnsOne(s => s.Assets, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Assets").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("AssetsCurrency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(s => s.Liabilities, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Liabilities").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("LiabilitiesCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasIndex(s => new { s.UserId, s.Date });
    }
}
