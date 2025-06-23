using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.User.Entities;

namespace Autopark.Web.Services;

public static class JwtEventsHandler
{
    public static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<AutoparkDbContext>();

        var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            context.Fail("Invalid user id");
            return;
        }

        var user = await dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == UserId.Create(userId));

        if (user == null)
        {
            context.Fail("User not found");
            return;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.ToString()));
        }

        var enterpriseIds = user.Roles.Any(r => r.Role == UserRoleType.Admin)
            ? await dbContext.Enterprises.Select(e => e.Id).ToListAsync()
            : await dbContext.ManagerEnterprises
                        .Where(me => me.Manager.UserId == user.Id)
                        .Select(me => me.EnterpriseId)
                        .ToListAsync();

        foreach (var enterpriseId in enterpriseIds)
            claims.Add(new Claim("enterprise", enterpriseId.Value.ToString()));

        // var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        // context.Principal = new ClaimsPrincipal(identity);
        foreach (var claim in claims)
            (context.Principal.Identity as ClaimsIdentity)?.AddClaim(claim);
    }
}