namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;
using WealthOS.Domain.ValueObjects;

public class FinancialGoal : Entity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = default!;
    public Money TargetAmount { get; private set; } = default!;
    public DateOnly TargetDate { get; private set; }

    /// <summary>Desired minimum percentage of monthly income to save (0-100). Drives task suggestions.</summary>
    public decimal TargetSavingsRatePercent { get; private set; }

    public bool IsActive { get; private set; } = true;

    private FinancialGoal() { }

    public FinancialGoal(Guid userId, string name, Money targetAmount, DateOnly targetDate, decimal targetSavingsRatePercent)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Goal name is required.", nameof(name));
        if (targetSavingsRatePercent is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(targetSavingsRatePercent), "Must be between 0 and 100.");

        UserId = userId;
        Name = name;
        TargetAmount = targetAmount;
        TargetDate = targetDate;
        TargetSavingsRatePercent = targetSavingsRatePercent;
    }

    public void Deactivate() => IsActive = false;
}
