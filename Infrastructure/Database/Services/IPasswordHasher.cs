namespace Autopark.Infrastructure.Database.Services;

public interface IPasswordHasher
{
    string HashPassword(string password, string salt);
    bool VerifyPassword(string password, string hashedPassword);
    bool VerifyPassword(string password, string salt, string hash);
    string GenerateSalt(int size = 16);
}