using MediatR;
using LanguageExt;

namespace Autopark.UseCases.BrandModel.Commands.Delete;

public record DeleteBrandModelCommand(
    int Id
) : IRequest<Fin<LanguageExt.Unit>>;