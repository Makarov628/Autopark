using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Driver.Commands.Create;

public record CreateDriverCommand(
    int UserId,
    decimal Salary,
    int EnterpriseId
) : IRequest<Fin<Unit>>;