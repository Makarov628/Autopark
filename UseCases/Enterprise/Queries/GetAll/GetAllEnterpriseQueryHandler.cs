using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.UseCases.Enterprise.Queries.GetAll;

internal class GetAllEnterprisesQueryHandler : IRequestHandler<GetAllEnterprisesQuery, Fin<List<EnterprisesResponse>>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetAllEnterprisesQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<List<EnterprisesResponse>>> Handle(GetAllEnterprisesQuery request, CancellationToken cancellationToken)
    {
        return await (
             from enterprise in _dbContext.Enterprises
             where _currentUser.EnterpriseIds.Contains(enterprise.Id)
             select new EnterprisesResponse(
                 enterprise.Id.Value,
                 enterprise.Name.Value,
                 enterprise.Address,
                 _dbContext.Vehicles.AsNoTracking()
                     .Where(v => v.EnterpriseId == enterprise.Id)
                     .Select(v => v.Id.Value)
                     .ToArray(),
                 _dbContext.Drivers.AsNoTracking()
                     .Where(d => d.EnterpriseId == enterprise.Id)
                     .Select(d => d.Id.Value)
                     .ToArray(),
                _dbContext.ManagerEnterprises.AsNoTracking()
                    .Where(me => me.EnterpriseId == enterprise.Id)
                    .Select(me => me.ManagerId.Value.ToString())
                    .ToArray()
             )
          ).ToListAsync(cancellationToken);
    }
}