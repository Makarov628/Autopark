using Autopark.Domain.Common.Models;
using Autopark.Domain.User.ValueObjects;
using System;

namespace Autopark.Domain.User.Entities;

public enum ActivationTokenType
{
    Email = 0,
    Phone = 1
}

public class ActivationToken : Entity<ActivationTokenId>
{
    public UserId UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public ActivationTokenType Type { get; set; }
    public virtual UserEntity User { get; set; }
}