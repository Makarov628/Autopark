using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
namespace Autopark.UseCases.Driver.Queries.GetAll;

internal class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, Fin<List<DriversResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllDriversQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<List<DriversResponse>>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
    {
        var drivers = await _dbContext.Drivers.AsNoTracking().ToListAsync(cancellationToken);
        return drivers.ConvertAll(driver => new DriversResponse(
            driver.Id.Value,
            driver.FirstName.Value,
            driver.LastName.Value,
            driver.DateOfBirth,
            driver.Salary,
            driver.EnterpriseId.Value,
            driver.VehicleId is not null ? driver.VehicleId.Value : null
        ));
    }
}