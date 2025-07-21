using MediatR;
using LanguageExt;

namespace Autopark.UseCases.Vehicle.Commands.Update;

public record UpdateVehicleCommand(
    int Id,
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color,
    string RegistrationNumber,
    int BrandModelId,
    int EnterpriseId,
    int? ActiveDriverId,
    DateTimeOffset? PurchaseDate = null
) : IRequest<Fin<LanguageExt.Unit>>;