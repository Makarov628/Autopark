using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class ActivationTokenConfiguration : IEntityTypeConfiguration<ActivationToken>
{
    public void Configure(EntityTypeBuilder<ActivationToken> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasConversion(
                id => id.Value,
                value => ActivationTokenId.Create(value))
            .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(a => a.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));
        builder.Property(a => a.Token)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(a => a.ExpiresAt)
            .IsRequired();
        builder.Property(a => a.Type)
            .IsRequired();
    }
}