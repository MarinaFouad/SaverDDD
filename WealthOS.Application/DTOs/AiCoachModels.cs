namespace WealthOS.Application.DTOs;

/// <summary>Everything the AI coach needs to reason about this week's tasks — no persistence types leak in.</summary>
public record AiCoachContextDto(
    decimal MonthlyIncome,
    decimal MonthlyExpenses,
    decimal MonthlySavings,
    decimal MonthlySurplus,
    decimal SavingsRatePercent,
    decimal TargetSavingsRatePercent,
    string? GoalName,
    decimal? GoalTargetAmount,
    IReadOnlyList<(string Category, decimal Amount)> ExpenseBreakdown
);

/// <summary>A single task suggestion as returned by the AI coach, before it's turned into a domain WeeklyTask.</summary>
public record AiSuggestedTaskDto(string Title, string Description, string Type);
