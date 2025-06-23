using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Manager.Queries.GetAll;

public record GetAllManagerQuery() : IRequest<Fin<List<ManagerResponse>>>;

public record ManagerResponse(
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