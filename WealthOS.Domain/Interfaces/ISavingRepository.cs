namespace WealthOS.Domain.Interfaces;

using WealthOS.Domain.Entities;

public interface ISavingRepository
{
    Task<SavingEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<SavingEntry>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<List<SavingEntry>> GetByUserInRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);
    Task AddAsync(SavingEntry entry, CancellationToken ct = default);
    void Remove(SavingEntry entry);
    Task SaveChangesAsync(CancellationToken ct = default);
}
