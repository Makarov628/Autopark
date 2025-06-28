using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Manager.Commands.Create;

public record CreateManagerCommand(
    int UserId,
    List<int> EnterpriseIds
) : IRequest<Fin<int>>;