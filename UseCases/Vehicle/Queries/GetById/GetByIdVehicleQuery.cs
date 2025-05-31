using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Vehicle.Queries.GetById;

public record GetByIdVehicleQuery(int Id) : IRequest<Fin<VehicleResponse>>;

public record VehicleResponse(
    int Id,
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color,
    string RegistrationNumber,
    int BrandModelId,
    int EnterpriseId,
    int? ActiveDriverId
);