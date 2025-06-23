using MediatR;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.User.Entities;
using LanguageExt;
using LanguageExt.Common;
using Autopark.Domain.Driver.ValueObjects;

namespace Autopark.UseCases.Driver.Queries.GetById;

public class GetByIdDriverQueryHandler : IRequestHandler<GetByIdDriverQuery, Fin<DriverResponse>>
{
    private readonly AutoparkDbContext _context;

    public GetByIdDriverQueryHandler(AutoparkDbContext context)
    {
        _context = context;
    }

    public async Task<Fin<DriverResponse>> Handle(GetByIdDriverQuery request, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers
            .Include(d => d.User)
            .Include(d => d.Enterprise)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == DriverId.Create(request.Id), cancellationToken);

        if (driver == null)
            return Error.New($"Driver not found with id: {request.Id}");

        return new DriverResponse(
            driver.Id.Value,
            driver.User.FirstName.Value,
            driver.User.LastName.Value,
            driver.User.DateOfBirth ?? DateTime.MinValue,
            driver.Salary,
            driver.EnterpriseId.Value,
            driver.VehicleId?.Value
        );
    }
}