using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Vehicle.Queries.GetAll;

public record GetAllVehiclesQuery() : IRequest<Fin<List<VehiclesResponse>>>;

public record VehiclesResponse(
    int Id,
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color,
    string RegistrationNumber,
    int BrandModelId,
    int EnterpriseId,
    int? ActiveDriverId,
    int[] DriverIds
);