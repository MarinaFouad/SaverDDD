namespace WealthOS.Domain.Interfaces;

using WealthOS.Domain.Entities;

public interface IGoalRepository
{
    Task<FinancialGoal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<FinancialGoal?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);
    Task<List<FinancialGoal>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(FinancialGoal goal, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
