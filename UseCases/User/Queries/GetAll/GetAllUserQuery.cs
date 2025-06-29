using LanguageExt;
using MediatR;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.User.Queries.GetAll;

public record GetAllUserQuery(UserRoleType? NotHasRole = null) : IRequest<Fin<List<UsersResponse>>>;

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