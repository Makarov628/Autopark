using Autopark.Domain.BrandModel.Entities;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Infrastructure.Database.Configurations;
using Autopark.Domain.User.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Autopark.Domain.Manager.Entities;

namespace Autopark.Infrastructure.Database;

public class AutoparkDbContext : DbContext
{
    public AutoparkDbContext(DbContextOptions<AutoparkDbContext> options)
        : base(options) { }

    public DbSet<VehicleEntity> Vehicles { get; set; } = null!;
    public DbSet<BrandModelEntity> BrandModels { get; set; } = null!;
    public DbSet<EnterpriseEntity> Enterprises { get; set; } = null!;
    public DbSet<DriverEntity> Drivers { get; set; } = null!;
    public DbSet<ManagerEntity> Managers { get; set; } = null!;
    public DbSet<ManagerEnterpriseEntity> ManagerEnterprises { get; set; } = null!;
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<Credentials> Credentials { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<Device> Devices { get; set; } = null!;
    public DbSet<ActivationToken> ActivationTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BrandModelConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new EnterpriseConfiguration());
        modelBuilder.ApplyConfiguration(new DriverConfiguration());
        modelBuilder.ApplyConfiguration(new ManagerConfiguration());
        modelBuilder.ApplyConfiguration(new ManagerEnterpriseConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CredentialsConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceConfiguration());
        modelBuilder.ApplyConfiguration(new ActivationTokenConfiguration());

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