namespace WealthOS.Application.Services;

using WealthOS.Domain.Entities;
using WealthOS.Domain.Enums;

/// <summary>
/// Turns this week's numbers (surplus, savings rate vs goal) into 2-4 concrete weekly tasks.
/// This is intentionally simple, rule-based logic for the MVP — a natural seam to later
/// swap in (or augment with) the AI coach mentioned as a phase-2 feature.
/// </summary>
public class WeeklyTaskSuggestionEngine
{
    public List<WeeklyTask> Suggest(
        Guid userId,
        DateOnly weekStartDate,
        decimal monthlySurplus,
        decimal savingsRatePercent,
        decimal targetSavingsRatePercent,
        decimal topExpenseCategoryShare,
        string? topExpenseCategoryName)
    {
        var tasks = new List<WeeklyTask>();

        // Behind on savings target -> push both levers.
        if (savingsRatePercent < targetSavingsRatePercent)
        {
            var gap = targetSavingsRatePercent - savingsRatePercent;

            tasks.Add(new WeeklyTask(
                userId,
                "Trim one recurring cost",
                gap >= 10
                    ? "Your savings rate is well below your goal. Cancel or downgrade one subscription or recurring expense this week."
                    : "Review last week's spending and cut one non-essential purchase to close the gap to your savings goal.",
                TaskType.ReduceExpenses,
                weekStartDate));

            tasks.Add(new WeeklyTask(
                userId,
                "Find one extra income source",
                "Spend 2 hours this week exploring a freelance gig, side task, or selling something you no longer need.",
                TaskType.IncreaseIncome,
                weekStartDate));
        }

        // Overspending outright -> deficit is the priority.
        if (monthlySurplus < 0)
        {
            tasks.Add(new WeeklyTask(
                userId,
                "Freeze non-essential spending",
                "You're spending more than you earn this month. Pause all non-essential purchases for the next 7 days.",
                TaskType.ReduceExpenses,
                weekStartDate));
        }

        // One category dominates spending -> call it out specifically.
        if (!string.IsNullOrWhiteSpace(topExpenseCategoryName) && topExpenseCategoryShare >= 40)
        {
            tasks.Add(new WeeklyTask(
                userId,
                $"Rein in {topExpenseCategoryName} spending",
                $"{topExpenseCategoryName} is eating {topExpenseCategoryShare:0}% of your expenses. Set a weekly cap for this category and track it daily.",
                TaskType.ReduceExpenses,
                weekStartDate));
        }

        // On track -> reinforce the habit instead of nagging.
        if (tasks.Count == 0)
        {
            tasks.Add(new WeeklyTask(
                userId,
                "Move this week's surplus into savings",
                "You're on pace for your goal. Transfer this week's surplus into your savings/investment account before you're tempted to spend it.",
                TaskType.BoostSavings,
                weekStartDate));
        }

        return tasks;
    }
}
