namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;
using WealthOS.Domain.Enums;

public class WeeklyTask : Entity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public TaskType Type { get; private set; }
    public TaskSource Source { get; private set; }
    public DateOnly WeekStartDate { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private WeeklyTask() { }

    public WeeklyTask(Guid userId, string title, string description, TaskType type, DateOnly weekStartDate, TaskSource source = TaskSource.RuleBased)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));

        UserId = userId;
        Update(title, description, type);
        Source = source;
        WeekStartDate = weekStartDate;
        IsCompleted = false;
    }

    /// <summary>Updates the editable fields of a task. Used by manual create/edit flows.</summary>
    public void Update(string title, string description, TaskType type)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
        Description = description ?? string.Empty;
        Type = type;
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void MarkIncomplete()
    {
        IsCompleted = false;
        CompletedAtUtc = null;
    }
}
