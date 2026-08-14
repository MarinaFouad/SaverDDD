namespace WealthOS.Domain.Interfaces;

using WealthOS.Domain.Entities;

public interface IIncomeRepository
{
    Task<IncomeEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<IncomeEntry>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<List<IncomeEntry>> GetByUserInRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);
    Task AddAsync(IncomeEntry entry, CancellationToken ct = default);
    void Remove(IncomeEntry entry);
    Task SaveChangesAsync(CancellationToken ct = default);
}
