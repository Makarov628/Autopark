using MediatR;

namespace Autopark.UseCases.User.Commands.Activate;

public class ActivateUserCommand : IRequest<bool>
{
    public string Token { get; set; }
    public string Password { get; set; }
    public string RepeatPassword { get; set; }
}