using Autopark.Domain.BrandModel.Entities;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Vehicle.Entities;
using Autopark.Domain.Vehicle.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class BrandModelConfiguration : IEntityTypeConfiguration<BrandModelEntity>
{
    public void Configure(EntityTypeBuilder<BrandModelEntity> builder)
    {
        ConfigureBrandModelsTable(builder);
    }

    public void ConfigureBrandModelsTable(EntityTypeBuilder<BrandModelEntity> builder)
    {
        builder.ToTable("BrandModels");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => BrandModelId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.HasMany(s => s.Vehicles)
            .WithOne(s => s.BrandModel)
            .HasForeignKey(s => s.BrandModelId)
            .HasPrincipalKey(s => s.Id);

        builder.Navigation(s => s.Vehicles).Metadata.SetField("_vehicles");
        builder.Navigation(s => s.Vehicles).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
