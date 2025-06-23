using System.Security.Claims;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.User.Entities;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.Web.Services;

public sealed class CurrentUser : ICurrentUser
{
    public bool IsAuthenticated { get; }
    public string Id { get; } = string.Empty;
    public string Login { get; } = string.Empty;
    public string Role { get; } = string.Empty;
    public IReadOnlyList<EnterpriseId> EnterpriseIds { get; } = Array.Empty<EnterpriseId>();
    public List<UserRoleType> Roles { get; } = new();

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var principal = accessor.HttpContext?.User;
        if (principal?.Identity is null || (!principal.Identity.IsAuthenticated)) return;

        IsAuthenticated = true;
        Id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        Login = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        // Получаем все роли пользователя
        var roleClaims = principal.FindAll(ClaimTypes.Role);
        Roles = roleClaims.Select(c => Enum.Parse<UserRoleType>(c.Value)).ToList();

        // Основная роль (первая в списке или пустая строка)
        Role = Roles.FirstOrDefault().ToString();

        // Получаем enterprise IDs для менеджеров
        if (Roles.Contains(UserRoleType.Manager) || Roles.Contains(UserRoleType.Admin))
        {
            EnterpriseIds = principal.FindAll("enterprise")
                                   .Select(c => EnterpriseId.Create(int.Parse(c.Value)))
                                   .ToArray();
        }
    }
}
