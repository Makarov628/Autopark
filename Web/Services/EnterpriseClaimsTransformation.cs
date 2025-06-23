
using System.Security.Claims;
using Autopark.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Manager.ValueObjects;
using Autopark.Domain.User.ValueObjects;

namespace Autopark.Web.Services;

public sealed class EnterpriseClaimsTransformation : IClaimsTransformation
{
    private readonly AutoparkDbContext _db;

    public EnterpriseClaimsTransformation(AutoparkDbContext db) => _db = db;

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (!principal.Identity?.IsAuthenticated ?? true) return principal;

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return principal;

        if (principal.Identity is not ClaimsIdentity id) return principal;

        // Удаляем существующие enterprise claims
        foreach (var c in id.FindAll("enterprise").ToArray())
            id.RemoveClaim(c);

        // Получаем enterprise IDs для менеджера
        var enterpriseIds = await _db.ManagerEnterprises
                              .Where(me => me.Manager.UserId == UserId.Create(int.Parse(userId)))
                              .Select(me => me.EnterpriseId)
                              .ToListAsync();

        foreach (var enterpriseId in enterpriseIds)
            id.AddClaim(new Claim("enterprise", enterpriseId.Value.ToString()));

        return principal;
    }
}