namespace WealthOS.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;

public class WealthOsDbContext : DbContext
{
    public WealthOsDbContext(DbContextOptions<WealthOsDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<IncomeEntry> Incomes => Set<IncomeEntry>();
    public DbSet<ExpenseEntry> Expenses => Set<ExpenseEntry>();
    public DbSet<SavingEntry> Savings => Set<SavingEntry>();
    public DbSet<FinancialGoal> Goals => Set<FinancialGoal>();
    public DbSet<WeeklyTask> WeeklyTasks => Set<WeeklyTask>();
    public DbSet<NetWorthSnapshot> NetWorthSnapshots => Set<NetWorthSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WealthOsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
