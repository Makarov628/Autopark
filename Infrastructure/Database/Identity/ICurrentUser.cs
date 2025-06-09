

using Autopark.Domain.Enterprise.ValueObjects;

namespace Autopark.Infrastructure.Database.Identity;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string Id { get; }
    string Login { get; }
    string Role { get; }
    IReadOnlyList<EnterpriseId> EnterpriseIds { get; }
}