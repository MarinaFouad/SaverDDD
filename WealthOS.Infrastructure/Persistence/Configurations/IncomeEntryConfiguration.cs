namespace WealthOS.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Entities;

public class IncomeEntryConfiguration : IEntityTypeConfiguration<IncomeEntry>
{
    public void Configure(EntityTypeBuilder<IncomeEntry> builder)
    {
        builder.ToTable("Incomes");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.UserId).IsRequired();
        builder.Property(i => i.Source).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(i => i.Date).IsRequired();
        builder.Property(i => i.Note).HasMaxLength(500);

        builder.OwnsOne(i => i.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });

        builder.HasIndex(i => new { i.UserId, i.Date });
    }
}
