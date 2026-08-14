namespace WealthOS.Domain.Interfaces;

using WealthOS.Domain.Entities;

public interface IWeeklyTaskRepository
{
    Task<WeeklyTask?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<WeeklyTask>> GetByUserForWeekAsync(Guid userId, DateOnly weekStartDate, CancellationToken ct = default);
    Task<List<WeeklyTask>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(WeeklyTask task, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<WeeklyTask> tasks, CancellationToken ct = default);
    void Remove(WeeklyTask task);
    Task SaveChangesAsync(CancellationToken ct = default);
}
