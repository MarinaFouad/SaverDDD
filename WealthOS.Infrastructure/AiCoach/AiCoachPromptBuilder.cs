namespace WealthOS.Infrastructure.AiCoach;

using System.Text;
using WealthOS.Application.DTOs;

/// <summary>Builds the shared prompt text and parses the shared JSON response shape used by every AI coach provider.</summary>
internal static class AiCoachPromptBuilder
{
    public const string SystemPrompt =
        "You are a pragmatic personal finance coach inside a budgeting app called WealthOS. " +
        "Given a user's monthly numbers, suggest 2 to 4 concrete, specific weekly tasks that will " +
        "increase their income, reduce their expenses, or boost their savings. " +
        "Be specific and actionable (reference actual numbers/categories given), not generic advice. " +
        "Respond with ONLY a JSON object of the exact shape: " +
        "{\"tasks\":[{\"title\":\"short imperative title\",\"description\":\"one to two sentences\",\"type\":\"IncreaseIncome|ReduceExpenses|BoostSavings\"}]}. " +
        "No markdown, no code fences, no extra commentary — JSON only.";

    public static string BuildUserPrompt(AiCoachContextDto ctx)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Monthly income: {ctx.MonthlyIncome:0}");
        sb.AppendLine($"Monthly expenses: {ctx.MonthlyExpenses:0}");
        sb.AppendLine($"Monthly savings: {ctx.MonthlySavings:0}");
        sb.AppendLine($"Monthly surplus (income - expenses): {ctx.MonthlySurplus:0}");
        sb.AppendLine($"Current savings rate: {ctx.SavingsRatePercent:0.0}%");
        sb.AppendLine($"Target savings rate: {ctx.TargetSavingsRatePercent:0.0}%");

        if (ctx.GoalName is not null)
            sb.AppendLine($"Active goal: \"{ctx.GoalName}\" target {ctx.GoalTargetAmount:0}");
        else
            sb.AppendLine("No active goal set.");

        if (ctx.ExpenseBreakdown.Count > 0)
        {
            sb.AppendLine("Expense breakdown this month:");
            foreach (var (category, amount) in ctx.ExpenseBreakdown.OrderByDescending(e => e.Amount))
                sb.AppendLine($"- {category}: {amount:0}");
        }

        return sb.ToString();
    }
}
