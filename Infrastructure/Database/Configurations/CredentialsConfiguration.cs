using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Autopark.Infrastructure.Database.Configurations;

public class CredentialsConfiguration : IEntityTypeConfiguration<Credentials>
{
    public void Configure(EntityTypeBuilder<Credentials> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(s => s.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn()
                    .HasConversion(
                        id => id.Value,
                        value => CredentialsId.Create(value))
                    .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        builder.Property(c => c.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.Create(value));

        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(c => c.PasswordHash)
            .HasMaxLength(512);
        builder.Property(c => c.Salt)
            .HasMaxLength(256);
        builder.Property(c => c.ProviderUserId)
            .HasMaxLength(256);
        builder.Property(c => c.ProviderEmail)
            .HasMaxLength(256);
    }
}