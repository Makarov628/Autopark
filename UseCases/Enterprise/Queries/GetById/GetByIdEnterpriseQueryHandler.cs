using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.Enterprise.ValueObjects;
using LanguageExt.Common;

namespace Autopark.UseCases.Enterprise.Queries.GetById;

internal class GetByIdEnterpriseQueryHandler : IRequestHandler<GetByIdEnterpriseQuery, Fin<EnterpriseResponse>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetByIdEnterpriseQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<EnterpriseResponse>> Handle(GetByIdEnterpriseQuery request, CancellationToken cancellationToken)
    {
        var enterpriseId = EnterpriseId.Create(request.Id);
        var enterprise = await _dbContext.Enterprises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == enterpriseId, cancellationToken);
        if (enterprise is null)
            return Error.New($"Enterprise not found with id: {request.Id}");

        return new EnterpriseResponse(
            enterprise.Id.Value,
            enterprise.Name.Value,
            enterprise.Address
        );
    }
}