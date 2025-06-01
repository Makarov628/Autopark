using Autopark.Domain.BrandModel.Enums;
using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Enterprise.Queries.GetById;

public record GetByIdEnterpriseQuery(int Id) : IRequest<Fin<EnterpriseResponse>>;

public record EnterpriseResponse(
    int Id,
    string Name,
    string Address,
    Vehicle[] Vehicles,
    Driver[] Drivers
);

public record BrandModel(
    int Id,
    string BrandName,
    string ModelName,
    TransportType TransportType,
    FuelType FuelType,
    uint SeatsNumber,
    uint MaximumLoadCapacityInKillograms
);

public record Vehicle(
    int Id,
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color,
    string RegistrationNumber,
    BrandModel BrandModel,
    Driver? ActiveDriver,
    Driver[] AttachedDrivers
);

public record VehicleForDriver(
    int Id,
    string Name,
    decimal Price,
    double MileageInKilometers,
    string Color,
    string RegistrationNumber,
    BrandModel BrandModel,
    int? ActiveDriverId
);

public record Driver(
    int Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    decimal Salary,
    VehicleForDriver? AttachedVehicle = null
);