using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class VehicleTrackPointConfiguration : IEntityTypeConfiguration<VehicleTrackPointEntity>
{
    public void Configure(EntityTypeBuilder<VehicleTrackPointEntity> builder)
    {
        ConfigureVehicleTrackPointsTable(builder);
    }

    private void ConfigureVehicleTrackPointsTable(EntityTypeBuilder<VehicleTrackPointEntity> builder)
    {
        builder.ToTable("VehicleTrackPoints");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => VehicleTrackPointId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.VehicleId)
            .ValueGeneratedNever()
            .HasConversion(
                vehicleId => vehicleId.Value,
                value => VehicleId.Create(value))
            .IsRequired();

        builder.Property(s => s.TimestampUtc)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(s => s.Location)
            .HasColumnType("geography")
            .IsRequired();

        builder.Property(s => s.Speed)
            .HasColumnType("real")
            .IsRequired();

        builder.Property(s => s.Rpm)
            .IsRequired();

        builder.Property(s => s.FuelLevel)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        // Индексы для быстрого поиска
        builder.HasIndex(s => new { s.VehicleId, s.TimestampUtc })
            .HasDatabaseName("IX_VehicleTrackPoints_VehicleId_TimestampUtc");

        builder.HasIndex(s => s.TimestampUtc)
            .HasDatabaseName("IX_VehicleTrackPoints_TimestampUtc");

        builder.HasIndex(s => s.VehicleId)
            .HasDatabaseName("IX_VehicleTrackPoints_VehicleId");

        // Связь с Vehicle
        builder.HasOne(s => s.Vehicle)
            .WithMany()
            .HasForeignKey(s => s.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}