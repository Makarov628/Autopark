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
        var enterprises = await _dbContext.Enterprises.AsNoTracking().ToListAsync(cancellationToken);
        return enterprises.ConvertAll(enterprise => new EnterprisesResponse(
            enterprise.Id.Value,
            enterprise.Name.Value,
            enterprise.Address
        ));
    }
}