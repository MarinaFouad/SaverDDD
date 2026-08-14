namespace WealthOS.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Entities;

public class SavingEntryConfiguration : IEntityTypeConfiguration<SavingEntry>
{
    public void Configure(EntityTypeBuilder<SavingEntry> builder)
    {
        builder.ToTable("Savings");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.Note).HasMaxLength(500);

        builder.OwnsOne(s => s.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });

        builder.HasIndex(s => new { s.UserId, s.Date });
    }
}
