namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class GoalRepository : IGoalRepository
{
    private readonly WealthOsDbContext _db;
    public GoalRepository(WealthOsDbContext db) => _db = db;

    public Task<FinancialGoal?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Goals.FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<FinancialGoal?> GetActiveByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.Goals.FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive, ct);

    public Task<List<FinancialGoal>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.Goals.Where(g => g.UserId == userId).ToListAsync(ct);

    public async Task AddAsync(FinancialGoal goal, CancellationToken ct = default) =>
        await _db.Goals.AddAsync(goal, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
