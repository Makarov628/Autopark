using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Enterprise.Commands.Update;

public record UpdateEnterpriseCommand(
    int Id,
    string Name,
    string Address
) : IRequest<Fin<Unit>>;