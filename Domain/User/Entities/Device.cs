using Autopark.Domain.Common.Models;
using Autopark.Domain.User.ValueObjects;
using System;

namespace Autopark.Domain.User.Entities;

public class Device : Entity<DeviceId>
{
    public UserId UserId { get; set; }
    public string DeviceName { get; set; }
    public DeviceType DeviceType { get; set; }
    public string RefreshTokenHash { get; set; }
    public string? PushToken { get; set; }
    public DateTime LastActive { get; set; }
    public DateTime ExpireDate { get; set; }
    public virtual UserEntity User { get; set; }
}