using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Driver.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Vehicle.ValueObjects;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.Driver.Commands.Update;

internal class UpdateDriverCommandHandler : IRequestHandler<UpdateDriverCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public UpdateDriverCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(UpdateDriverCommand request, CancellationToken cancellationToken)
    {
        var driverId = DriverId.Create(request.Id);
        var driver = await _dbContext.Drivers
            .Include(d => d.User)
            .FirstOrDefaultAsync(v => v.Id == driverId, cancellationToken);

        if (driver is null)
            return Error.New($"Driver not found with id: {request.Id}");

        var enterpriseId = EnterpriseId.Create(request.EnterpriseId);
        var enterpriseExists = await _dbContext.Enterprises.AnyAsync(e => e.Id == enterpriseId, cancellationToken);
        if (!enterpriseExists)
            return Error.New($"Предприятие с идентификатором '{enterpriseId.Value}' не существует");

        var vehicleId = request.AttachedVehicleId.HasValue
            ? VehicleId.Create(request.AttachedVehicleId.Value)
            : null;

        if (vehicleId is not null && !await _dbContext.Vehicles.AnyAsync(v => v.Id == vehicleId && v.EnterpriseId == enterpriseId, cancellationToken))
            return Error.New($"Автомобиль с идентификатором '{vehicleId.Value}' не существует или не может быть назначен этому водителю");

        // var lastName = CyrillicString.Create(request.LastName);
        // var firstName = CyrillicString.Create(request.FirstName);

        // var potentialErrors = new Either<Error, ValueObject>[]
        // {
        //     lastName.ToValueObjectEither(),
        //     firstName.ToValueObjectEither()
        // };

        // var aggregatedErrorMessage = potentialErrors
        //     .MapLeftT(error => error.Message)
        //     .Lefts()
        //     .JoinStrings("; ");

        // if (!aggregatedErrorMessage.IsNullOrEmpty())
        //     return Error.New(aggregatedErrorMessage);

        // Обновляем данные пользователя
        // driver.User.FirstName = firstName.Head();
        // driver.User.LastName = lastName.Head();
        // driver.User.DateOfBirth = request.DateOfBirth;

        // Обновляем данные водителя
        driver.UpdateEnterprise(enterpriseId);
        driver.UpdateSalary(request.Salary);
        if (vehicleId is not null)
        {
            driver.AttachToVehicle(vehicleId);
        }
        else
        {
            driver.DetachFromVehicle();
        }

        _dbContext.Drivers.Attach(driver);
        _dbContext.Entry(driver).State = EntityState.Modified;
        // _dbContext.Entry(driver.User).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return LanguageExt.Unit.Default;
    }
}