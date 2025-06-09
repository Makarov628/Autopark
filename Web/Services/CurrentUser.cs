

using System.Security.Claims;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.Web.Services;

public sealed class CurrentUser : ICurrentUser
{
    public bool IsAuthenticated { get; }
    public string Id { get; }
    public string Login { get; } = string.Empty;
    public string Role { get; } = string.Empty;
    public IReadOnlyList<EnterpriseId> EnterpriseIds { get; } = Array.Empty<EnterpriseId>();

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var principal = accessor.HttpContext?.User;
        if (principal?.Identity is null || (!principal.Identity.IsAuthenticated)) return;

        IsAuthenticated = true;
        Id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        Login = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        Role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        EnterpriseIds = principal.FindAll("enterprise")
                                 .Select(c => EnterpriseId.Create(int.Parse(c.Value)))
                                 .ToArray();
    }
}
