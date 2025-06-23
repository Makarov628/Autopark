using MediatR;

namespace Autopark.UseCases.User.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
{
    public string RefreshToken { get; set; }
}

public class RefreshTokenResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int DeviceId { get; set; }
}