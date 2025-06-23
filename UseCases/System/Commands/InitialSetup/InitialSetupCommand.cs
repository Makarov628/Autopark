using MediatR;

namespace Autopark.UseCases.System.Commands.InitialSetup;

public record InitialSetupCommand(
    string Email,
    string FirstName,
    string LastName,
    string Phone,
    string Password
) : IRequest<InitialSetupResponse>;