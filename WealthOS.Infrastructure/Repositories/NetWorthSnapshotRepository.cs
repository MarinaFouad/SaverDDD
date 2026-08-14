namespace WealthOS.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WealthOS.Domain.Entities;
using WealthOS.Domain.Interfaces;
using WealthOS.Infrastructure.Persistence;

public class NetWorthSnapshotRepository : INetWorthSnapshotRepository
{
    private readonly WealthOsDbContext _db;
    public NetWorthSnapshotRepository(WealthOsDbContext db) => _db = db;

    public Task<List<NetWorthSnapshot>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        _db.NetWorthSnapshots.Where(s => s.UserId == userId).OrderBy(s => s.Date).ToListAsync(ct);

    public Task<NetWorthSnapshot?> GetLatestAsync(Guid userId, CancellationToken ct = default) =>
        _db.NetWorthSnapshots.Where(s => s.UserId == userId).OrderByDescending(s => s.Date).FirstOrDefaultAsync(ct);

    public async Task AddAsync(NetWorthSnapshot snapshot, CancellationToken ct = default) =>
        await _db.NetWorthSnapshots.AddAsync(snapshot, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
