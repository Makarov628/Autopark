using MediatR;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.User.Commands.Login;

public class LoginUserCommand : IRequest<LoginUserResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string DeviceName { get; set; }
    public DeviceType DeviceType { get; set; }
}

public class LoginUserResponse
{
    public int UserId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int DeviceId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}