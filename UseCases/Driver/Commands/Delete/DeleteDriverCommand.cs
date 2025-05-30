using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Driver.Commands.Delete;

public record DeleteDriverCommand(
    int Id
) : IRequest<Fin<LanguageExt.Unit>>;