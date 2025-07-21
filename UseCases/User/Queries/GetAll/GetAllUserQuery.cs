using LanguageExt;
using MediatR;
using Autopark.Domain.User.Entities;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.User.Queries.GetAll;

public record GetAllUserQuery(
    UserRoleType? NotHasRole = null,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,
    string? SortDirection = null,
    UserRoleType? Role = null,
    string? Search = null
) : IRequest<Fin<PagedResult<UsersResponse>>>;

public record UsersResponse(
    int Id,
    string Email,
    string Phone,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    bool IsActive,
    List<UserRoleType> Roles,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);