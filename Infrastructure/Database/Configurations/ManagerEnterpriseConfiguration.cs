using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class ManagerEnterpriseConfiguration : IEntityTypeConfiguration<ManagerEnterpriseEntity>
{
    public void Configure(EntityTypeBuilder<ManagerEnterpriseEntity> builder)
    {
        ConfigureVehiclesTable(builder);
    }

    public void ConfigureVehiclesTable(EntityTypeBuilder<ManagerEnterpriseEntity> builder)
    {
        builder.ToTable("ManagerEnterprises");
        builder.HasKey(s => new { s.ManagerId, s.EnterpriseId });

        builder.Property(s => s.EnterpriseId)
            .ValueGeneratedNever()
            .HasConversion(
                enterpriseId => enterpriseId.Value,
                value => EnterpriseId.Create(value));

        builder.HasOne(s => s.Enterprise)
            .WithMany(s => s.EnterpriseManagers)
            .HasForeignKey(s => s.EnterpriseId)
            .HasPrincipalKey(s => s.Id);

        builder.HasOne(s => s.Manager)
            .WithMany(s => s.EnterpriseManagers)
            .HasForeignKey(s => s.ManagerId)
            .HasPrincipalKey(s => s.Id);
    }
}
