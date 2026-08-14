namespace WealthOS.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Entities;

public class FinancialGoalConfiguration : IEntityTypeConfiguration<FinancialGoal>
{
    public void Configure(EntityTypeBuilder<FinancialGoal> builder)
    {
        builder.ToTable("Goals");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.UserId).IsRequired();
        builder.Property(g => g.Name).IsRequired().HasMaxLength(200);
        builder.Property(g => g.TargetDate).IsRequired();
        builder.Property(g => g.TargetSavingsRatePercent).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(g => g.IsActive).IsRequired();

        builder.OwnsOne(g => g.TargetAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TargetAmount").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("TargetCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasIndex(g => new { g.UserId, g.IsActive });
    }
}
