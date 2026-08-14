namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class WeeklyTaskRepository : IWeeklyTaskRepository
{
    private readonly WealthOsDbContext _db;
    public WeeklyTaskRepository(WealthOsDbContext db) => _db = db;

    public Task<WeeklyTask?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.WeeklyTasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<List<WeeklyTask>> GetByUserForWeekAsync(Guid userId, DateOnly weekStartDate, CancellationToken ct = default) =>
        _db.WeeklyTasks.Where(t => t.UserId == userId && t.WeekStartDate == weekStartDate).ToListAsync(ct);

    public Task<List<WeeklyTask>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.WeeklyTasks.Where(t => t.UserId == userId).OrderByDescending(t => t.WeekStartDate).ToListAsync(ct);

    public async Task AddAsync(WeeklyTask task, CancellationToken ct = default) =>
        await _db.WeeklyTasks.AddAsync(task, ct);

    public async Task AddRangeAsync(IEnumerable<WeeklyTask> tasks, CancellationToken ct = default) =>
        await _db.WeeklyTasks.AddRangeAsync(tasks, ct);

    public void Remove(WeeklyTask task) => _db.WeeklyTasks.Remove(task);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
