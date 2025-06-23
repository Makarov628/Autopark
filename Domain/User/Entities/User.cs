using Autopark.Domain.User.ValueObjects;
using System;
using System.Collections.Generic;

namespace Autopark.Domain.User.Entities;

public class User
{
    public UserId Id { get; protected set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? EmailConfirmed { get; set; }
    public DateTime? PhoneConfirmed { get; set; }

    public virtual Credentials Credentials { get; set; }
    public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    // Конструкторы, методы и т.д. можно добавить позже
}