
using Autopark.Domain.Enterprise.Entities;
using Autopark.Domain.Enterprise.ValueObjects;

namespace Autopark.Domain.Manager.Entities;

public class ManagerEnterpriseEntity
{
    public string ManagerId { get; protected set; }
    public ManagerEntity Manager { get; protected set; }

    public EnterpriseId EnterpriseId { get; protected set; }
    public EnterpriseEntity Enterprise { get; protected set; }

    private ManagerEnterpriseEntity(string managerId, EnterpriseId enterpriseId)
    {
        ManagerId = managerId;
        EnterpriseId = enterpriseId;
    }

    public static ManagerEnterpriseEntity Create(string managerId, EnterpriseId enterpriseId) => new(managerId, enterpriseId);

    protected ManagerEnterpriseEntity() { }
}
