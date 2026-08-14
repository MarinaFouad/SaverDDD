namespace WealthOS.Domain.Interfaces;

using WealthOS.Domain.Entities;

public interface IExpenseRepository
{
    Task<ExpenseEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ExpenseEntry>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<List<ExpenseEntry>> GetByUserInRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);
    Task AddAsync(ExpenseEntry entry, CancellationToken ct = default);
    void Remove(ExpenseEntry entry);
    Task SaveChangesAsync(CancellationToken ct = default);
}
