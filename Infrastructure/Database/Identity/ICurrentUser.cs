using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.User.Entities;

namespace Autopark.Infrastructure.Database.Identity;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string Id { get; }
    string Login { get; }
    string Role { get; }
    List<UserRoleType> Roles { get; }
    IReadOnlyList<EnterpriseId> EnterpriseIds { get; }
}