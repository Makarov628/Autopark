using MediatR;

namespace Autopark.UseCases.User.Commands.SetPushToken;

public class SetPushTokenCommand : IRequest<bool>
{
    public int DeviceId { get; set; }
    public string PushToken { get; set; }
}