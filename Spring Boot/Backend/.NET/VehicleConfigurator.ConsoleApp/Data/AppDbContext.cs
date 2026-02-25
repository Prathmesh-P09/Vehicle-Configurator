using Microsoft.EntityFrameworkCore;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<SgMfgMaster> SgMfgMasters { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<AlternateComponentMaster> AlternateComponentMasters { get; set; }
        public DbSet<VehicleDetail> VehicleDetails { get; set; }
        public DbSet<VehicleDefaultConfig> VehicleDefaultConfigs { get; set; }
        public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Enum conversions if necessary, though [EnumDataType] attribute helps too.
            // For CompType, we used String in POCO, so it maps directly.
            // For InvoiceStatus, we used Enum. Let's ensure string storage if DB expects strings (ENUM('Pending',...)).
            
            modelBuilder.Entity<InvoiceHeader>()
                .Property(e => e.Status)
                .HasConversion<string>(); // Convert Enum to String for DB storage
        }
    }
}
