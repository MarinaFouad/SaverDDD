namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;
using WealthOS.Domain.Enums;
using WealthOS.Domain.ValueObjects;

public class ExpenseEntry : Entity
{
    public Guid UserId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public ExpenseCategory Category { get; private set; }
    public string? Note { get; private set; }
    public DateOnly Date { get; private set; }

    private ExpenseEntry() { }

    public ExpenseEntry(Guid userId, Money amount, ExpenseCategory category, DateOnly date, string? note = null)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));
        if (amount.Amount <= 0) throw new ArgumentException("Expense amount must be positive.", nameof(amount));

        UserId = userId;
        Amount = amount;
        Category = category;
        Date = date;
        Note = note;
    }
}
