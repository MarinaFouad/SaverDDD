namespace WealthOS.Domain.Services;

using WealthOS.Domain.ValueObjects;

/// <summary>
/// Pure domain service holding the core WealthOS math: monthly surplus, savings rate,
/// and a simple net-worth projection. No I/O, no persistence — just business rules,
/// so it's trivially unit-testable and reusable by both the dashboard and the
/// weekly-task-suggestion logic.
/// </summary>
public class FinancialCalculator
{
    /// <summary>
    /// Monthly surplus = income - expenses. Returned as plain decimal (not Money) because
    /// Money's invariant forbids negative amounts, and a deficit is a valid, meaningful result here.
    /// </summary>
    public decimal CalculateMonthlySurplus(Money monthlyIncome, Money monthlyExpenses)
        => monthlyIncome.Amount - monthlyExpenses.Amount;

    /// <summary>Savings rate = savings-this-month / income-this-month * 100. Returns 0 if income is 0.</summary>
    public decimal CalculateSavingsRatePercent(Money monthlyIncome, Money monthlySavings)
    {
        if (monthlyIncome.Amount == 0) return 0m;
        return Math.Round(monthlySavings.Amount / monthlyIncome.Amount * 100m, 2);
    }

    /// <summary>
    /// Simple linear projection: assumes the given monthly surplus continues unchanged,
    /// accumulating into net worth every month for <paramref name="months"/> months.
    /// Deliberately avoids modeling interest/returns for the MVP — good enough to show
    /// a directional "where you're headed" line on the dashboard.
    /// </summary>
    public IReadOnlyList<(int MonthOffset, decimal ProjectedNetWorth)> ProjectNetWorth(
        decimal currentNetWorth, decimal monthlySurplus, int months = 12)
    {
        var result = new List<(int, decimal)>(months + 1) { (0, currentNetWorth) };
        var running = currentNetWorth;
        for (var m = 1; m <= months; m++)
        {
            running += monthlySurplus;
            result.Add((m, running));
        }
        return result;
    }

    /// <summary>How many months until the goal's target amount is reached at the current surplus pace.</summary>
    public int? MonthsToGoal(decimal currentNetWorth, decimal targetAmount, decimal monthlySurplus)
    {
        if (currentNetWorth >= targetAmount) return 0;
        if (monthlySurplus <= 0) return null; // unreachable at current pace
        return (int)Math.Ceiling((targetAmount - currentNetWorth) / monthlySurplus);
    }
}
