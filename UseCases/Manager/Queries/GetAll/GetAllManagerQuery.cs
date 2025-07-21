using LanguageExt;
using MediatR;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.Manager.Queries.GetAll;

public record GetAllManagerQuery(
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    string? SortDirection = null,
    string? Search = null
) : IRequest<Fin<PagedResult<ManagersResponse>>>;

public record ManagersResponse(
    int Id,
    int UserId,
    string Email,
    string Phone,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    bool IsActive,
    List<int> EnterpriseIds,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);