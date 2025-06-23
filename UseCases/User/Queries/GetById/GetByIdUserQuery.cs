using LanguageExt;
using MediatR;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.User.Queries.GetById;

public record GetByIdUserQuery(
    int Id
) : IRequest<Fin<UserResponse>>;

public record UserResponse(
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