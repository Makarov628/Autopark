using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Enterprise.Commands.Delete;

public record DeleteEnterpriseCommand(
    int Id
) : IRequest<Fin<LanguageExt.Unit>>;