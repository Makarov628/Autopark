using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<ManagerEntity>
{
    public void Configure(EntityTypeBuilder<ManagerEntity> builder)
    {
        ConfigureVehiclesTable(builder);
    }

    public void ConfigureVehiclesTable(EntityTypeBuilder<ManagerEntity> builder)
    {
        builder.Property(s => s.LastName)
            .ValueGeneratedNever()
            .HasConversion(
                name => name.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.Property(s => s.FirstName)
            .ValueGeneratedNever()
            .HasConversion(
                name => name.Value,
                value => CyrillicString.Create(value).ThrowIfFail());

        builder.HasMany(s => s.EnterpriseManagers)
            .WithOne(s => s.Manager)
            .HasForeignKey(s => s.ManagerId)
            .HasPrincipalKey(s => s.Id);

        builder.Navigation(s => s.EnterpriseManagers).Metadata.SetField("_enterpriseManagers");
        builder.Navigation(s => s.EnterpriseManagers).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
