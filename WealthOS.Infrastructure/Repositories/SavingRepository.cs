namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class SavingRepository : ISavingRepository
{
    private readonly WealthOsDbContext _db;
    public SavingRepository(WealthOsDbContext db) => _db = db;

    public Task<SavingEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Savings.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<List<SavingEntry>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.Savings.Where(s => s.UserId == userId).ToListAsync(ct);

    public Task<List<SavingEntry>> GetByUserInRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default) =>
        _db.Savings.Where(s => s.UserId == userId && s.Date >= from && s.Date <= to).ToListAsync(ct);

    public async Task AddAsync(SavingEntry entry, CancellationToken ct = default) =>
        await _db.Savings.AddAsync(entry, ct);

    public void Remove(SavingEntry entry) => _db.Savings.Remove(entry);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
