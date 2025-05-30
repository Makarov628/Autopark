using Autopark.Domain.BrandModel.Entities;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Autopark.Infrastructure.Database;

public class AutoparkDbContext : DbContext
{
    public AutoparkDbContext(DbContextOptions<AutoparkDbContext> options)
        : base(options) { }

    public DbSet<VehicleEntity> Vehicles { get; set; } = null!;
    public DbSet<BrandModelEntity> BrandModels { get; set; } = null!;
    public DbSet<EnterpriseEntity> Enterprises { get; set; } = null!;
    public DbSet<DriverEntity> Drivers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BrandModelConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new EnterpriseConfiguration());
        modelBuilder.ApplyConfiguration(new DriverConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges()
    {
        UpdateDates();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        UpdateDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateDates()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e.Entity is BaseEntity && (e.State is EntityState.Added || e.State is EntityState.Modified));

        foreach (var entry in entries)
        {
            if (entry.State is EntityState.Added)
                ((BaseEntity)entry.Entity).SetCreatedDate();

            ((BaseEntity)entry.Entity).RenewUpdateDate();
        }
    }
}