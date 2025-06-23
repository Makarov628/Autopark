using BCrypt.Net;
using System.Security.Cryptography;

namespace Autopark.Infrastructure.Database.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public bool VerifyPassword(string password, string salt, string hash)
    {
        // Для совместимости: если hash уже содержит salt, то salt можно игнорировать
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GenerateSalt(int size = 16)
    {
        var saltBytes = new byte[size];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return BCrypt.Net.BCrypt.GenerateSalt(12);
    }
}