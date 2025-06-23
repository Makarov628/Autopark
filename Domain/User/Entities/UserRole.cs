using Autopark.Domain.Common.Models;
using Autopark.Domain.User.ValueObjects;
using System;

namespace Autopark.Domain.User.Entities;

public enum UserRoleType
{
    Admin = 0,
    Manager = 1,
    Driver = 2
    // Можно добавить другие роли позже
}

public class UserRole : Entity<UserRoleId>
{
    public UserId UserId { get; set; }
    public UserRoleType Role { get; set; }
    public virtual UserEntity User { get; set; }
}