using MediatR;

namespace Autopark.UseCases.User.Queries.GetActivationToken;

public class GetActivationTokenQuery : IRequest<GetActivationTokenResponse>
{
    public string Email { get; set; } = string.Empty;
}

public class GetActivationTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public bool Success { get; set; }
}