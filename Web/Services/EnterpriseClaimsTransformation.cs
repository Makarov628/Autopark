

namespace Autopark.Web.Services;

using System.Security.Claims;
using Autopark.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

public sealed class EnterpriseClaimsTransformation : IClaimsTransformation
{
    private readonly AutoparkDbContext _db;

    public EnterpriseClaimsTransformation(AutoparkDbContext db) => _db = db;

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (!principal.Identity?.IsAuthenticated ?? true) return principal;

        var uid = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (principal.Identity is not ClaimsIdentity id) return principal;
        foreach (var c in id.FindAll("enterprise").ToArray())
            id.RemoveClaim(c);

        var enterpriseIds = await _db.ManagerEnterprises
                              .Where(me => me.ManagerId == uid)
                              .Select(me => me.EnterpriseId)
                              .ToListAsync();

        foreach (var enterpriseId in enterpriseIds)
            id.AddClaim(new Claim("enterprise", enterpriseId.Value.ToString()));

        return principal;
    }
}