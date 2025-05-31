using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Driver.ValueObjects;
using LanguageExt.Common;

namespace Autopark.UseCases.Driver.Queries.GetById;

internal class GetByIdDriverQueryHandler : IRequestHandler<GetByIdDriverQuery, Fin<DriverResponse>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetByIdDriverQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<DriverResponse>> Handle(GetByIdDriverQuery request, CancellationToken cancellationToken)
    {
        var driverId = DriverId.Create(request.Id);
        var driver = await _dbContext.Drivers.AsNoTracking().FirstOrDefaultAsync(d => d.Id == driverId, cancellationToken);
        if (driver is null)
            return Error.New($"Driver not found with id: {request.Id}");

        return new DriverResponse(
            driver.Id.Value,
            driver.FirstName.Value,
            driver.LastName.Value,
            driver.DateOfBirth,
            driver.Salary,
            driver.EnterpriseId.Value,
            driver.VehicleId is not null ? driver.VehicleId.Value : null
        );
    }
}