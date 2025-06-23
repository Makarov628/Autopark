using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Driver.Queries.GetAll;

public record GetAllDriversQuery() : IRequest<Fin<List<DriversResponse>>>;

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