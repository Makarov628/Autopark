using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<TripEntity>
{
    public void Configure(EntityTypeBuilder<TripEntity> builder)
    {
        ConfigureTripsTable(builder);
    }

    private void ConfigureTripsTable(EntityTypeBuilder<TripEntity> builder)
    {
        builder.ToTable("Trips");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => TripId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.VehicleId)
            .ValueGeneratedNever()
            .HasConversion(
                vehicleId => vehicleId.Value,
                value => VehicleId.Create(value));

        builder.Property(s => s.StartUtc)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(s => s.EndUtc)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(s => s.DistanceKm)
            .HasColumnType("float")
            .IsRequired(false);

        builder.Property(s => s.StartPointId)
            .HasConversion(
                id => id != null ? id.Value : (long?)null,
                value => value.HasValue ? TripPointId.Create(value.Value) : null)
            .IsRequired(false);

        builder.Property(s => s.EndPointId)
            .HasConversion(
                id => id != null ? id.Value : (long?)null,
                value => value.HasValue ? TripPointId.Create(value.Value) : null)
            .IsRequired(false);

        builder.Property(s => s.CreatedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        // Связи с точками
        builder.HasOne(s => s.StartPoint)
            .WithMany()
            .HasForeignKey(s => s.StartPointId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.EndPoint)
            .WithMany()
            .HasForeignKey(s => s.EndPointId)
            .OnDelete(DeleteBehavior.NoAction);

        // Индексы для быстрого поиска по диапазону времени
        builder.HasIndex(s => new { s.VehicleId, s.StartUtc, s.EndUtc });
        builder.HasIndex(s => s.StartUtc);
        builder.HasIndex(s => s.EndUtc);
    }
}