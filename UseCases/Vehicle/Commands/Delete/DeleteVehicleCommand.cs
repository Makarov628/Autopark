using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Vehicle.Commands.Delete;

public record DeleteVehicleCommand(
    int Id
) : IRequest<Fin<LanguageExt.Unit>>;