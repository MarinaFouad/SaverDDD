namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly WealthOsDbContext _db;
    public UserRepository(WealthOsDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetFirstAsync(CancellationToken ct = default) =>
        _db.Users.OrderBy(u => u.CreatedAtUtc).FirstOrDefaultAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await _db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
