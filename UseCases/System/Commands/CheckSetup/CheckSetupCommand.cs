using MediatR;

namespace Autopark.UseCases.System.Commands.CheckSetup;

public record CheckSetupCommand : IRequest<CheckSetupResponse>;