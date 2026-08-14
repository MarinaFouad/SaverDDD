namespace WealthOS.Domain.Interfaces;

using WealthOS.Domain.Entities;

public interface INetWorthSnapshotRepository
{
    Task<List<NetWorthSnapshot>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<NetWorthSnapshot?> GetLatestAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(NetWorthSnapshot snapshot, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
