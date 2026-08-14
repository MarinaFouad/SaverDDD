namespace WealthOS.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WealthOS.Domain.Entities;

public class WeeklyTaskConfiguration : IEntityTypeConfiguration<WeeklyTask>
{
    public void Configure(EntityTypeBuilder<WeeklyTask> builder)
    {
        builder.ToTable("WeeklyTasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(1000);
        builder.Property(t => t.Type).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(t => t.Source).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.WeekStartDate).IsRequired();
        builder.Property(t => t.IsCompleted).IsRequired();

        builder.HasIndex(t => new { t.UserId, t.WeekStartDate });
    }
}
