namespace WealthOS.Domain.Entities;

using WealthOS.Domain.Common;

/// <summary>
/// Single-user MVP aggregate root anchor. Multi-tenant auth can be layered on later
/// without changing the shape of Income/Expense/Saving/Goal/Task entities (they all key off UserId).
/// </summary>
public class User : Entity
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }

    private User() { }

    public User(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));

        Name = name;
        Email = email;
        CreatedAtUtc = DateTime.UtcNow;
    }
}
