namespace Autopark.Infrastructure.Database.Services;

public interface ITokenGenerator
{
    string GenerateActivationToken();
    string GenerateRefreshToken();
    string GenerateToken(int size = 32);
}