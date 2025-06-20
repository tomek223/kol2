using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using ProgramModel = WebApplication1.Models.Program;

namespace WebApplication1.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<WashingMachine> WashingMachines { get; set; }
    public DbSet<ProgramModel> Programs { get; set; }
    public DbSet<AvailableProgram> AvailablePrograms { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PurchaseHistory> PurchaseHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WashingMachine>().HasKey(w => w.WashingMachineId);
        modelBuilder.Entity<ProgramModel>().HasKey(p => p.ProgramId);
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);

        modelBuilder.Entity<AvailableProgram>().HasKey(ap => ap.AvailableProgramId);
        modelBuilder.Entity<AvailableProgram>()
            .HasOne(ap => ap.Program)
            .WithMany(p => p.AvailablePrograms)
            .HasForeignKey(ap => ap.ProgramId);
        modelBuilder.Entity<AvailableProgram>()
            .HasOne(ap => ap.WashingMachine)
            .WithMany(w => w.AvailablePrograms)
            .HasForeignKey(ap => ap.WashingMachineId);

        modelBuilder.Entity<PurchaseHistory>().HasKey(ph => new { ph.AvailableProgramId, ph.CustomerId, ph.PurchaseDate });
        modelBuilder.Entity<PurchaseHistory>()
            .HasOne(ph => ph.AvailableProgram)
            .WithMany(ap => ap.Purchases)
            .HasForeignKey(ph => ph.AvailableProgramId);
        modelBuilder.Entity<PurchaseHistory>()
            .HasOne(ph => ph.Customer)
            .WithMany(c => c.Purchases)
            .HasForeignKey(ph => ph.CustomerId);

        modelBuilder.Entity<WashingMachine>()
            .HasIndex(w => w.SerialNumber).IsUnique();

        modelBuilder.Entity<ProgramModel>()
            .HasIndex(p => p.Name).IsUnique();
    }
}
