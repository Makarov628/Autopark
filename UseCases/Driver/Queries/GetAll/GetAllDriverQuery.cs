using LanguageExt;
using MediatR;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.Driver.Queries.GetAll;

public record GetAllDriversQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    string? SortDirection = null,
    int? EnterpriseId = null,
    string? Search = null
) : IRequest<Fin<PagedResult<DriversResponse>>>;

public record DriversResponse(
    int Id,
    int UserId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    decimal Salary,
    int EnterpriseId,
    int? AttachedVehicleId
);