namespace WealthOS.Application.DTOs;

public record GoalDto(Guid Id, string Name, decimal TargetAmount, DateOnly TargetDate, decimal TargetSavingsRatePercent, bool IsActive);

public record CreateGoalDto(string Name, decimal TargetAmount, DateOnly TargetDate, decimal TargetSavingsRatePercent);
