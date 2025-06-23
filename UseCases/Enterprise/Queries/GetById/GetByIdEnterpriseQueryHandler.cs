using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Enterprise.ValueObjects;
using LanguageExt.Common;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.UseCases.Enterprise.Queries.GetById;

internal class GetByIdEnterpriseQueryHandler : IRequestHandler<GetByIdEnterpriseQuery, Fin<EnterpriseResponse>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetByIdEnterpriseQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<EnterpriseResponse>> Handle(GetByIdEnterpriseQuery request, CancellationToken cancellationToken)
    {
        var enterpriseId = EnterpriseId.Create(request.Id);
        var enterprise = await _dbContext.Enterprises
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == enterpriseId && _currentUser.EnterpriseIds.Contains(e.Id), cancellationToken);

        if (enterprise is null)
            return Error.New($"Enterprise not found with id: {request.Id}");

        var vehiclesIds = await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.EnterpriseId == enterpriseId)
            .Select(v => v.Id)
            .ToListAsync(cancellationToken);

        var driversIds = await _dbContext.Drivers
            .AsNoTracking()
            .Where(d => d.EnterpriseId == enterpriseId)
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        var managersIds = await _dbContext.ManagerEnterprises
            .AsNoTracking()
            .Where(me => me.EnterpriseId == enterpriseId)
            .Select(me => me.ManagerId)
            .ToListAsync(cancellationToken);

        return new EnterpriseResponse(
            enterprise.Id.Value,
            enterprise.Name.Value,
            enterprise.Address,
            vehiclesIds.Select(v => v.Value).ToArray(),
            driversIds.Select(d => d.Value).ToArray(),
            managersIds.Select(m => m.Value.ToString()).ToArray()
        );
    }
}