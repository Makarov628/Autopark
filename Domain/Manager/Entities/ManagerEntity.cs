using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.User.Entities;
using Autopark.Domain.Manager.ValueObjects;
using System.Collections.Generic;

namespace Autopark.Domain.Manager.Entities;

public class ManagerEntity : Entity<ManagerId>
{
    public UserId UserId { get; set; }
    public virtual UserEntity User { get; set; }

    private readonly List<ManagerEnterpriseEntity> _enterpriseManagers = new();

    public IReadOnlyList<ManagerEnterpriseEntity> EnterpriseManagers => _enterpriseManagers.AsReadOnly();
}