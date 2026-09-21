using Microsoft.EntityFrameworkCore;
using WarrantRisk.Domain;

namespace WarrantRisk.Api;

public sealed class WarrantService(WarrantDbContext db)
{
    public Task<List<Warrant>> SearchAsync(string? keyword) => db.Warrants.AsNoTracking().Where(x => string.IsNullOrWhiteSpace(keyword) || x.WarrantId.Contains(keyword)).OrderBy(x => x.WarrantId).ToListAsync();
    public async Task<(TrialResult Result, TrialLog Log)> TrialAsync(string id, decimal marketPrice)
    {
        var warrant = await db.Warrants.SingleOrDefaultAsync(x => x.WarrantId == id) ?? throw new KeyNotFoundException("找不到權證。");
        var result = WarrantCalculator.Calculate(warrant, marketPrice);
        var log = new TrialLog { WarrantId = id, MarketPrice = result.MarketPrice, TheoryPrice = result.TheoryPrice, HedgeQty = result.HedgeQty, CreatedTime = DateTime.Now };
        db.TrialLogs.Add(log); await db.SaveChangesAsync(); return (result, log);
    }
    public Task<List<TrialLog>> RecentAsync(string id) => db.TrialLogs.AsNoTracking().Where(x => x.WarrantId == id).OrderByDescending(x => x.CreatedTime).ThenByDescending(x => x.LogId).Take(10).ToListAsync();
}
