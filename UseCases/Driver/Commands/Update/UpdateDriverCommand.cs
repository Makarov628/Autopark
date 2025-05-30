using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace Autopark.UseCases.Driver.Commands.Update;

public record UpdateDriverCommand(
    int Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    decimal Salary,
    int EnterpriseId,
    int? AttachedVehicleId
) : IRequest<Fin<Unit>>;