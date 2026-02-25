using Microsoft.EntityFrameworkCore;
using project_vc_.Models;

namespace project_vc_.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Segment> Segments { get; set; }
    public DbSet<SgMfgMaster> SgMfgMasters { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<AlternateComponentMaster> AlternateComponentMasters { get; set; }
    public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<VehicleDefaultConfig> VehicleDefaultConfigs { get; set; }
    public DbSet<VehicleDetail> VehicleDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Additional configuration if needed
        modelBuilder.Entity<SgMfgMaster>()
            .HasOne(s => s.Mfg)
            .WithMany()
            .HasForeignKey(s => s.MfgId);

        modelBuilder.Entity<SgMfgMaster>()
            .HasOne(s => s.Seg)
            .WithMany()
            .HasForeignKey(s => s.SegId);
            
         modelBuilder.Entity<AlternateComponentMaster>()
            .HasOne(a => a.Comp)
            .WithMany(c => c.AlternateComponentMasters)
            .HasForeignKey(a => a.CompId);

         modelBuilder.Entity<AlternateComponentMaster>()
            .HasOne(a => a.AltComp)
            .WithMany() 
            .HasForeignKey(a => a.AltCompId);
    }
}
