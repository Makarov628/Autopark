using Autopark.Domain.Common.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Autopark.Domain.Manager.Entities;

public class ManagerEntity : IdentityUser
{
    private readonly List<ManagerEnterpriseEntity> _enterpriseManagers = new();

    public bool IsPasswordInitialized { get; set; } = true;
    public CyrillicString FirstName { get; set; }
    public CyrillicString LastName { get; set; }

    public IReadOnlyList<ManagerEnterpriseEntity> EnterpriseManagers => _enterpriseManagers.AsReadOnly();
}