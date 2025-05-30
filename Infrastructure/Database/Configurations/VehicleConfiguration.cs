using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<VehicleEntity>
{
    public void Configure(EntityTypeBuilder<VehicleEntity> builder)
    {
        ConfigureVehiclesTable(builder);
    }

    public void ConfigureVehiclesTable(EntityTypeBuilder<VehicleEntity> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => VehicleId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.Name)
            .ValueGeneratedNever()
            .HasConversion(
                name => name.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.Property(s => s.Color)
            .ValueGeneratedNever()
            .HasConversion(
                registrationNumber => registrationNumber.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.Property(s => s.Price)
            .ValueGeneratedNever()
            .HasConversion(
                price => price.Value,
                value => Price.Create(value).ThrowIfFail());

        builder.Property(s => s.MileageInKilometers)
            .ValueGeneratedNever()
            .HasConversion(
                mileage => mileage.ValueInKilometers,
                value => Mileage.Create(value).ThrowIfFail());

        builder.Property(s => s.RegistrationNumber)
            .ValueGeneratedNever()
            .HasConversion(
                registrationNumber => registrationNumber.Value,
                value => RegistrationNumber.Create(value).ThrowIfFail());

        builder.Property(s => s.BrandModelId)
            .ValueGeneratedNever()
            .HasConversion(
                brandModelId => brandModelId.Value,
                value => BrandModelId.Create(value));

        builder.Property(s => s.EnterpriseId)
            .ValueGeneratedNever()
            .HasConversion(
                enterpriseId => enterpriseId.Value,
                value => EnterpriseId.Create(value));

        builder.Property(s => s.ActiveDriverId)
            .ValueGeneratedNever()
            .HasConversion(
                activeDriverId => activeDriverId == null ? null : (int?)activeDriverId.Value,
                value => value.HasValue ? DriverId.Create(value.Value) : null);

        builder.HasOne(s => s.BrandModel)
            .WithMany(s => s.Vehicles)
            .HasForeignKey(s => s.BrandModelId)
            .HasPrincipalKey(s => s.Id);

        builder.HasOne(s => s.Enterprise)
            .WithMany(s => s.Vehicles)
            .HasForeignKey(s => s.EnterpriseId)
            .HasPrincipalKey(s => s.Id);

        builder.HasMany(s => s.Drivers)
            .WithOne(s => s.Vehicle)
            .HasForeignKey(s => s.VehicleId)
            .HasPrincipalKey(s => s.Id);

        builder.Navigation(s => s.Drivers).Metadata.SetField("_drivers");
        builder.Navigation(s => s.Drivers).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
