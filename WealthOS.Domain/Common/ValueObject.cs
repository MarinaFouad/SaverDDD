namespace WealthOS.Domain.Common;

/// <summary>Base class for DDD value objects. Structural equality.</summary>
public abstract class ValueObject
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other || GetType() != other.GetType()) return false;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents().Aggregate(17, (hash, c) => hash * 31 + (c?.GetHashCode() ?? 0));
}
