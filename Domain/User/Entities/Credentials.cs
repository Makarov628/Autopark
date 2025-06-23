using Autopark.Domain.Common.Models;
using Autopark.Domain.User.ValueObjects;
using System;

namespace Autopark.Domain.User.Entities;

public enum CredentialsType
{
    Local = 0,
    Google = 1,
    // Можно добавить другие типы позже
}

public class Credentials : Entity<CredentialsId>
{
    public UserId UserId { get; set; }
    public CredentialsType Type { get; set; }

    // Для Local
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }

    // Для OAuth
    public string? ProviderUserId { get; set; }
    public string? ProviderEmail { get; set; }

    public virtual UserEntity User { get; set; }
}