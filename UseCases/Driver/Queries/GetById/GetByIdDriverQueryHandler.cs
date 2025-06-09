using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Driver.ValueObjects;
using LanguageExt.Common;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.UseCases.Driver.Queries.GetById;

internal class GetByIdDriverQueryHandler : IRequestHandler<GetByIdDriverQuery, Fin<DriverResponse>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetByIdDriverQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<DriverResponse>> Handle(GetByIdDriverQuery request, CancellationToken cancellationToken)
    {
        var driverId = DriverId.Create(request.Id);
        var driver = await _dbContext.Drivers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == driverId && _currentUser.EnterpriseIds.Contains(d.EnterpriseId), cancellationToken);

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