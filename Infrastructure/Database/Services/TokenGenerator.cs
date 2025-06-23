using System;
using System.Security.Cryptography;

namespace Autopark.Infrastructure.Database.Services;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateActivationToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "")
            .Substring(0, 32);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public string GenerateToken(int size = 32)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(size))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}