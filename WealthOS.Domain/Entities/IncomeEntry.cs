namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;
using WealthOS.Domain.Enums;
using WealthOS.Domain.ValueObjects;

public class IncomeEntry : Entity
{
    public Guid UserId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public IncomeSource Source { get; private set; }
    public string? Note { get; private set; }
    public DateOnly Date { get; private set; }

    private IncomeEntry() { }

    public IncomeEntry(Guid userId, Money amount, IncomeSource source, DateOnly date, string? note = null)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (amount.Amount <= 0) throw new ArgumentException("Income amount must be positive.", nameof(amount));

        UserId = userId;
        Amount = amount;
        Source = source;
        Date = date;
        Note = note;
    }
}
