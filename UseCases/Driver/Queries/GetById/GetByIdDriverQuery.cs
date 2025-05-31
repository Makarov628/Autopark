using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Driver.Queries.GetById;

public record GetByIdDriverQuery(int Id) : IRequest<Fin<DriverResponse>>;

public record DriverResponse(
    int Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    decimal Salary,
    int EnterpriseId,
    int? AttachedVehicleId
);