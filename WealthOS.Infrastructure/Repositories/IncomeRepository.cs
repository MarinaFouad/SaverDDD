namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class IncomeRepository : IIncomeRepository
{
    private readonly WealthOsDbContext _db;
    public IncomeRepository(WealthOsDbContext db) => _db = db;

    public Task<IncomeEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Incomes.FirstOrDefaultAsync(i => i.Id == id, ct);

    public Task<List<IncomeEntry>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.Incomes.Where(i => i.UserId == userId).ToListAsync(ct);

    public Task<List<IncomeEntry>> GetByUserInRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default) =>
        _db.Incomes.Where(i => i.UserId == userId && i.Date >= from && i.Date <= to).ToListAsync(ct);

    public async Task AddAsync(IncomeEntry entry, CancellationToken ct = default) =>
        await _db.Incomes.AddAsync(entry, ct);

    public void Remove(IncomeEntry entry) => _db.Incomes.Remove(entry);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
