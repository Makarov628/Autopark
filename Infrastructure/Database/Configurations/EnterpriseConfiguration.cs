

using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Enterprise.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class EnterpriseConfiguration : IEntityTypeConfiguration<EnterpriseEntity>
{
    public void Configure(EntityTypeBuilder<EnterpriseEntity> builder)
    {
        ConfigureEnterprisesTable(builder);
    }

    private void ConfigureEnterprisesTable(EntityTypeBuilder<EnterpriseEntity> builder)
    {
        builder.ToTable("Enterprises");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => Domain.Enterprise.ValueObjects.EnterpriseId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.Name)
            .ValueGeneratedNever()
            .HasConversion(
                name => name.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.Property(s => s.Address)
            .ValueGeneratedNever()
            .HasConversion(
                address => address,
                value => value);

        builder.HasMany(s => s.Vehicles)
            .WithOne(s => s.Enterprise)
            .HasForeignKey(s => s.EnterpriseId)
            .HasPrincipalKey(s => s.Id);

        builder.HasMany(s => s.Drivers)
            .WithOne(s => s.Enterprise)
            .HasForeignKey(s => s.EnterpriseId)
            .HasPrincipalKey(s => s.Id);

        builder.HasMany(s => s.EnterpriseManagers)
            .WithOne(s => s.Enterprise)
            .HasForeignKey(s => s.EnterpriseId)
            .HasPrincipalKey(s => s.Id);

        builder.Navigation(s => s.EnterpriseManagers).Metadata.SetField("_enterpriseManagers");
        builder.Navigation(s => s.EnterpriseManagers).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(s => s.Vehicles).Metadata.SetField("_vehicles");
        builder.Navigation(s => s.Vehicles).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(s => s.Drivers).Metadata.SetField("_drivers");
        builder.Navigation(s => s.Drivers).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}