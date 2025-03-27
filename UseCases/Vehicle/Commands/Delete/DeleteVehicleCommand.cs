using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Vehicle.Commands.Delete;

public record DeleteVehicleCommand(
    Guid Id
) : IRequest<Fin<LanguageExt.Unit>>;