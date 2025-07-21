using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Vehicle.Commands.Create;

public record CreateVehicleCommand(
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color,
    string RegistrationNumber,
    int BrandModelId,
    int EnterpriseId,
    DateTimeOffset? PurchaseDate = null
) : IRequest<Fin<Unit>>;