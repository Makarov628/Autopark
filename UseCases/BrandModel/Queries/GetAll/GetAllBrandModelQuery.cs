using Autopark.Domain.BrandModel.Enums;
using LanguageExt;
using MediatR;

namespace Autopark.UseCases.BrandModel.Queries.GetAll;

public record GetAllBrandModelQuery() : IRequest<Fin<List<BrandModelResponse>>>;

public record BrandModelResponse(
    int Id,
    string BrandName,
    string ModelName,
    TransportType TransportType,
    FuelType FuelType,
    uint SeatsNumber,
    uint MaximumLoadCapacityInKillograms
);