using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Driver.Commands.Create;

public record CreateDriverCommand(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    decimal Salary,
    int EnterpriseId
) : IRequest<Fin<Unit>>;