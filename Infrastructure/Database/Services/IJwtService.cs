using System.Security.Claims;
using Autopark.Domain.User.Entities;
using System.Collections.Generic;
using Autopark.Domain.User.ValueObjects;

namespace Autopark.Infrastructure.Database.Services;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);
    ClaimsPrincipal ValidateToken(string token);
    string GenerateAccessToken(int userId, int deviceId);
    string GenerateRefreshToken();
}