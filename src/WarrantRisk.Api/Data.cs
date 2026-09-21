using Microsoft.EntityFrameworkCore;
using WarrantRisk.Domain;

namespace WarrantRisk.Api;

public sealed class WarrantDbContext(DbContextOptions<WarrantDbContext> options) : DbContext(options)
{
    public DbSet<Warrant> Warrants => Set<Warrant>();
    public DbSet<TrialLog> TrialLogs => Set<TrialLog>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Warrant>(e => { e.ToTable("Warrant_Master"); e.HasKey(x => x.WarrantId); e.Property(x => x.WarrantId).HasColumnName("Warrant_ID").HasMaxLength(10).IsUnicode(false); e.Property(x => x.StrikePrice).HasColumnName("Strike_Price").HasPrecision(18,4); e.Property(x => x.ConversionRatio).HasColumnName("Conversion_Ratio").HasPrecision(18,4); e.Property(x => x.WarrantType).HasColumnName("Warrant_Type").HasMaxLength(4).IsUnicode(false); e.Property(x => x.PositionQty).HasColumnName("Position_Qty"); });
        modelBuilder.Entity<TrialLog>(e => { e.ToTable("Warrant_Trial_Log"); e.HasKey(x => x.LogId); e.Property(x => x.LogId).HasColumnName("Log_ID"); e.Property(x => x.WarrantId).HasColumnName("Warrant_ID").HasMaxLength(10).IsUnicode(false); e.Property(x => x.MarketPrice).HasColumnName("Market_Price").HasPrecision(18,4); e.Property(x => x.TheoryPrice).HasColumnName("Theory_Price").HasPrecision(18,4); e.Property(x => x.HedgeQty).HasColumnName("Hedge_Qty").HasPrecision(18,2); e.Property(x => x.CreatedTime).HasColumnName("Created_Time"); });
    }
}
