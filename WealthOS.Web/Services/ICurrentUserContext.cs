namespace WealthOS.Web.Services;

public interface ICurrentUserContext
{
    /// <summary>
    /// Returns the id of the single demo/MVP user, creating it on first use.
    /// Swap this out for real ASP.NET Core Identity / auth once multi-user support is needed —
    /// nothing in Application or Domain depends on how a user id is resolved.
    /// </summary>
    Task<Guid> GetCurrentUserIdAsync(CancellationToken ct = default);
}
