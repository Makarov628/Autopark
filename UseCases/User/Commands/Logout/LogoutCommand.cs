using MediatR;

namespace Autopark.UseCases.User.Commands.Logout;

public class LogoutCommand : IRequest<bool>
{
    public int DeviceId { get; set; }
}