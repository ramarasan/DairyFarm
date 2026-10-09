using DairyFarm.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DairyFarm.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Cow> Cows => Set<Cow>();
    public DbSet<MilkRecord> MilkRecords => Set<MilkRecord>();
    public DbSet<DoctorVisit> DoctorVisits => Set<DoctorVisit>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<LoadUnloadTxn> LoadUnloads => Set<LoadUnloadTxn>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasKey(x => x.UserId);
        b.Entity<Cow>().HasKey(x => x.CowId);
        b.Entity<MilkRecord>().HasKey(x => x.MilkRecordId);
        b.Entity<DoctorVisit>().HasKey(x => x.VisitId);
        b.Entity<StockItem>().HasKey(x => x.ItemId);
        // Table has an AFTER INSERT trigger that updates stock
        b.Entity<LoadUnloadTxn>().ToTable("LoadUnload", t => t.HasTrigger("trg_LoadUnload_Stock"))
            .HasKey(x => x.TxnId);
        b.Entity<Cow>().Property(x => x.PurchasePrice).HasPrecision(12, 2);
        b.Entity<MilkRecord>().Property(x => x.Litres).HasPrecision(8, 2);
        b.Entity<MilkRecord>().Property(x => x.Fat).HasPrecision(4, 2);
        b.Entity<DoctorVisit>().Property(x => x.Cost).HasPrecision(10, 2);
        b.Entity<StockItem>().Property(x => x.CurrentQty).HasPrecision(12, 2);
        b.Entity<StockItem>().Property(x => x.ReorderLevel).HasPrecision(12, 2);
        b.Entity<LoadUnloadTxn>().Property(x => x.Quantity).HasPrecision(12, 2);
    }
}
