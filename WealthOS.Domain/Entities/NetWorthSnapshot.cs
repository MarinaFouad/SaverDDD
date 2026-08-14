namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;
using WealthOS.Domain.ValueObjects;

/// <summary>Point-in-time net worth reading, used to chart history and build projections.</summary>
public class NetWorthSnapshot : Entity
{
    public Guid UserId { get; private set; }
    public DateOnly Date { get; private set; }
    public Money Assets { get; private set; } = default!;
    public Money Liabilities { get; private set; } = default!;

    private NetWorthSnapshot() { }

    public NetWorthSnapshot(Guid userId, DateOnly date, Money assets, Money liabilities)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId is required.", nameof(userId));

        UserId = userId;
        Date = date;
        Assets = assets;
        Liabilities = liabilities;
    }

    public decimal NetWorthAmount => Assets.Amount - Liabilities.Amount;
}
