using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.User.ValueObjects;
using System;
using System.Collections.Generic;

namespace Autopark.Domain.User.Entities;

public class UserEntity : Entity<UserId>
{
    public string Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? EmailConfirmed { get; set; }
    public DateTime? PhoneConfirmed { get; set; }
    public CyrillicString FirstName { get; set; }
    public CyrillicString LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public virtual Credentials Credentials { get; set; }
    public virtual ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    // Конструкторы, методы и т.д. можно добавить позже
}