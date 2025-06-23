using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Autopark.Domain.Manager.ValueObjects;
using Autopark.Domain.User.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class ManagerConfiguration : IEntityTypeConfiguration<ManagerEntity>
{
    public void Configure(EntityTypeBuilder<ManagerEntity> builder)
    {
        ConfigureManagersTable(builder);
    }

    public void ConfigureManagersTable(EntityTypeBuilder<ManagerEntity> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(s => s.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn()
                    .HasConversion(
                        id => id.Value,
                        value => ManagerId.Create(value))
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(m => m.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.EnterpriseManagers)
            .WithOne(s => s.Manager)
            .HasForeignKey(s => s.ManagerId)
            .HasPrincipalKey(s => s.Id);

        builder.Navigation(s => s.EnterpriseManagers).Metadata.SetField("_enterpriseManagers");
        builder.Navigation(s => s.EnterpriseManagers).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
