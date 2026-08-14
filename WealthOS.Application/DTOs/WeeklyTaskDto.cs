namespace WealthOS.Application.DTOs;

using WealthOS.Domain.Enums;

public record WeeklyTaskDto(Guid Id, string Title, string Description, TaskType Type, TaskSource Source, DateOnly WeekStartDate, bool IsCompleted);

public record CreateWeeklyTaskDto(string Title, string Description, TaskType Type);

public record UpdateWeeklyTaskDto(string Title, string Description, TaskType Type);
