using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Autopark.Domain.Manager.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class ManagerEnterpriseConfiguration : IEntityTypeConfiguration<ManagerEnterpriseEntity>
{
    public void Configure(EntityTypeBuilder<ManagerEnterpriseEntity> builder)
    {
        ConfigureManagerEnterprisesTable(builder);
    }

    public void ConfigureManagerEnterprisesTable(EntityTypeBuilder<ManagerEnterpriseEntity> builder)
    {
        builder.ToTable("ManagerEnterprises");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn()
                    .HasConversion(
                        id => id.Value,
                        value => ManagerEnterpriseEntityId.Create(value))
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.ManagerId)
            .HasConversion(
                id => id.Value,
                value => ManagerId.Create(value));

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
