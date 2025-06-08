using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.Enterprise.Queries.GetAll;

internal class GetAllEnterprisesQueryHandler : IRequestHandler<GetAllEnterprisesQuery, Fin<List<EnterprisesResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllEnterprisesQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<List<EnterprisesResponse>>> Handle(GetAllEnterprisesQuery request, CancellationToken cancellationToken)
    {
        return await (
             from enterprise in _dbContext.Enterprises
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
                     .ToArray()
             )
          ).ToListAsync(cancellationToken);
    }
}