using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Enterprise.Commands.Create;

public record CreateEnterpriseCommand(
    string Name,
    string Address
) : IRequest<Fin<Unit>>;