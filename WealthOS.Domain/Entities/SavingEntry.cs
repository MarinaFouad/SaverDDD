namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;
using WealthOS.Domain.ValueObjects;

/// <summary>A deposit toward savings/net worth (e.g. a transfer into a savings account or investment).</summary>
public class SavingEntry : Entity
{
    public Guid UserId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public string? Note { get; private set; }
    public DateOnly Date { get; private set; }

    private SavingEntry() { }

    public SavingEntry(Guid userId, Money amount, DateOnly date, string? note = null)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (amount.Amount <= 0) throw new ArgumentException("Saving amount must be positive.", nameof(amount));

        UserId = userId;
        Amount = amount;
        Date = date;
        Note = note;
    }
}
