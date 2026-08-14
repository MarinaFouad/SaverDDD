namespace WealthOS.Web.Services;

using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IUserRepository _userRepository;
    private static Guid? _cachedUserId; // fine for a single-tenant MVP; remove once real auth lands

    public CurrentUserContext(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<Guid> GetCurrentUserIdAsync(CancellationToken ct = default)
    {
        if (_cachedUserId is { } cached) return cached;

        var user = await _userRepository.GetFirstAsync(ct);
        if (user is null)
        {
            user = new User("Demo User", "demo@wealthos.local");
            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);
        }

        _cachedUserId = user.Id;
        return user.Id;
    }
}
