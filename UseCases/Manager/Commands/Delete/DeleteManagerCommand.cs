using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Manager.Commands.Delete;

public record DeleteManagerCommand(
    int Id
) : IRequest<Fin<LanguageExt.Unit>>;