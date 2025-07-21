using LanguageExt;
using MediatR;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.Vehicle.Queries.GetAll;

public record GetAllVehiclesQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    string? SortDirection = null,
    int? EnterpriseId = null,
    int? BrandModelId = null,
    string? Search = null
) : IRequest<Fin<PagedResult<VehiclesResponse>>>;

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
    DateTimeOffset? PurchaseDate,
    int[] DriverIds
);