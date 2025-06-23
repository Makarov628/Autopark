using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(s => s.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn()
                    .HasConversion(
                        id => id.Value,
                        value => UserId.Create(value))
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Phone)
            .HasMaxLength(32);

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.EmailConfirmed);
        builder.Property(u => u.PhoneConfirmed);

        builder.Property(u => u.FirstName)
            .HasConversion(
                name => name.Value,
                value => Autopark.Domain.Common.ValueObjects.CyrillicString.Create(value).ThrowIfFail());

        builder.Property(u => u.LastName)
            .HasConversion(
                name => name.Value,
                value => Autopark.Domain.Common.ValueObjects.CyrillicString.Create(value).ThrowIfFail());

        builder.Property(u => u.DateOfBirth);

        builder.HasOne(u => u.Credentials)
            .WithOne(c => c.User)
            .HasForeignKey<Credentials>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Roles)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Devices)
            .WithOne(d => d.User)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}