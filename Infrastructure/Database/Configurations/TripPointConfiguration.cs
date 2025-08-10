using Autopark.Domain.Trip.Entities;
using Autopark.Domain.Trip.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class TripPointConfiguration : IEntityTypeConfiguration<TripPointEntity>
{
    public void Configure(EntityTypeBuilder<TripPointEntity> builder)
    {
        builder.ToTable("TripPoints");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => TripPointId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(s => s.Location)
            .HasColumnType("geography")
            .IsRequired();

        builder.Property(s => s.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(s => s.AddressResolvedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired(false);

        builder.Property(s => s.CreatedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        // Индексы
        // Для geography колонки нужен пространственный индекс, который создается отдельно
        // builder.HasIndex(s => s.Location) - не работает для geography

        builder.HasIndex(s => s.CreatedAt)
            .HasDatabaseName("IX_TripPoints_CreatedAt");
    }
}