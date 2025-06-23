using Autopark.Domain.Common.Models;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Manager.ValueObjects;

namespace Autopark.Domain.Manager.Entities;

public class ManagerEnterpriseEntity : Entity<ManagerEnterpriseEntityId>
{
    public ManagerId ManagerId { get; protected set; }
    public ManagerEntity Manager { get; protected set; }

    public EnterpriseId EnterpriseId { get; protected set; }
    public EnterpriseEntity Enterprise { get; protected set; }

    private ManagerEnterpriseEntity(ManagerId managerId, EnterpriseId enterpriseId)
    {
        ManagerId = managerId;
        EnterpriseId = enterpriseId;
    }

    public static ManagerEnterpriseEntity Create(ManagerId managerId, EnterpriseId enterpriseId) => new(managerId, enterpriseId);

    protected ManagerEnterpriseEntity() { }
}
