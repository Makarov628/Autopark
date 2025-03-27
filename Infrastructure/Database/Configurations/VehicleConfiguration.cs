using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
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
        builder.HasKey("Id");
        builder.Property(s => s.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => VehicleId.Create(value)
            );
    }
}
