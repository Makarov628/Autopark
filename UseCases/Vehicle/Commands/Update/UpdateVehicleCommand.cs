using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Vehicle.Commands.Update;

public record UpdateVehicleCommand(
    Guid Id,
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color
) : IRequest<Fin<LanguageExt.Unit>>;