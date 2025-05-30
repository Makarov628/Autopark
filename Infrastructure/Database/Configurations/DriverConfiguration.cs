

using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<DriverEntity>
{
    public void Configure(EntityTypeBuilder<DriverEntity> builder)
    {
        ConfigureDriversTable(builder);
    }

    private void ConfigureDriversTable(EntityTypeBuilder<DriverEntity> builder)
    {
        builder.ToTable("Drivers");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => DriverId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.FirstName)
            .ValueGeneratedNever()
            .HasConversion(
                firstName => firstName.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.Property(s => s.LastName)
            .ValueGeneratedNever()
            .HasConversion(
                lastName => lastName.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.Property(s => s.VehicleId)
            .ValueGeneratedNever()
            .HasConversion(
                vehicleId => vehicleId == null ? null : (int?)vehicleId.Value,
                value => value.HasValue ? VehicleId.Create(value.Value) : null);

        builder.Property(s => s.EnterpriseId)
            .ValueGeneratedNever()
            .HasConversion(
                enterpriseId => enterpriseId.Value,
                value => EnterpriseId.Create(value));

        builder.HasOne(s => s.Enterprise)
            .WithMany(s => s.Drivers)
            .HasForeignKey(s => s.EnterpriseId)
            .HasPrincipalKey(s => s.Id);

        builder.HasOne(s => s.Vehicle)
            .WithMany(s => s.Drivers)
            .HasForeignKey(s => s.VehicleId)
            .HasPrincipalKey(s => s.Id);
    }
}