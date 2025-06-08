using Autopark.Domain.BrandModel.Enums;
using LanguageExt;
using MediatR;

namespace Autopark.UseCases.BrandModel.Queries.GetById;

public record GetByIdBrandModelQuery(
    int Id
) : IRequest<Fin<BrandModelResponse>>;

public record BrandModelResponse(
    int Id,
    string BrandName,
    string ModelName,
    TransportType TransportType,
    FuelType FuelType,
    uint SeatsNumber,
    uint MaximumLoadCapacityInKillograms,
    int[] VehicleIds
);