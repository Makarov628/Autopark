using LanguageExt;
using MediatR;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.Enterprise.Queries.GetAll;

public record GetAllEnterprisesQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    string? SortDirection = null,
    string? Search = null
) : IRequest<Fin<PagedResult<EnterprisesResponse>>>;

public record EnterprisesResponse(
    int Id,
    string Name,
    string Address,
    string? TimeZoneId,
    int[] VehicleIds,
    int[] DriverIds,
    string[] ManagerIds
);