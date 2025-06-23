using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;
using System.Collections.Generic;

namespace Autopark.UseCases.Manager.Commands.Create;

public record UpdateManagerEnterprisesCommand(
    int UserId,
    List<int> EnterpriseIds
) : IRequest<Fin<Unit>>;