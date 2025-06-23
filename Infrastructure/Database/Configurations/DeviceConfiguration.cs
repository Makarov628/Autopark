using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(s => s.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn()
                    .HasConversion(
                        id => id.Value,
                        value => DeviceId.Create(value))
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(d => d.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));
        builder.Property(d => d.DeviceName)
            .IsRequired()
            .HasMaxLength(128);
        builder.Property(d => d.DeviceType)
            .IsRequired();
        builder.Property(d => d.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(512);
        builder.Property(d => d.PushToken)
            .HasMaxLength(256);
        builder.Property(d => d.LastActive)
            .IsRequired();
        builder.Property(d => d.ExpireDate)
            .IsRequired();
    }
}