namespace WealthOS.Application.DTOs;

public record DashboardDto(
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    decimal MonthlySavings,
    decimal MonthlySurplus,
    decimal SavingsRatePercent,
    decimal CurrentNetWorth,
    decimal? GoalTargetAmount,
    string? GoalName,
    int? MonthsToGoal,
    decimal GoalProgressPercent,
    IReadOnlyList<ProjectionPointDto> Projection,
    IReadOnlyList<WeeklyTaskDto> ThisWeekTasks
);

public record ProjectionPointDto(int MonthOffset, decimal ProjectedNetWorth);
