using MediatR;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.User.Entities;
using LanguageExt;

namespace Autopark.UseCases.Driver.Queries.GetAll;

public class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, Fin<List<DriversResponse>>>
{
    private readonly AutoparkDbContext _context;

    public GetAllDriversQueryHandler(AutoparkDbContext context)
    {
        _context = context;
    }

    public async Task<Fin<List<DriversResponse>>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
    {
        var drivers = await _context.Drivers
            .Include(d => d.User)
            .Include(d => d.Enterprise)
            .Include(d => d.Vehicle)
            .ToListAsync(cancellationToken);

        return drivers.Select(driver => new DriversResponse(
            driver.Id.Value,
            driver.UserId.Value,
            driver.User.FirstName.Value,
            driver.User.LastName.Value,
            driver.User.DateOfBirth ?? DateTime.MinValue,
            driver.Salary,
            driver.EnterpriseId.Value,
            driver.VehicleId?.Value
        )).ToList();
    }
}