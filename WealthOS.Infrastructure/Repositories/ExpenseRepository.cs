namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class ExpenseRepository : IExpenseRepository
{
    private readonly WealthOsDbContext _db;
    public ExpenseRepository(WealthOsDbContext db) => _db = db;

    public Task<ExpenseEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Expenses.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<List<ExpenseEntry>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.Expenses.Where(e => e.UserId == userId).ToListAsync(ct);

    public Task<List<ExpenseEntry>> GetByUserInRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default) =>
        _db.Expenses.Where(e => e.UserId == userId && e.Date >= from && e.Date <= to).ToListAsync(ct);

    public async Task AddAsync(ExpenseEntry entry, CancellationToken ct = default) =>
        await _db.Expenses.AddAsync(entry, ct);

    public void Remove(ExpenseEntry entry) => _db.Expenses.Remove(entry);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
