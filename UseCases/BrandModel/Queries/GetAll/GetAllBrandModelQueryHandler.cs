using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database.Identity;
namespace Autopark.UseCases.BrandModel.Queries.GetAll;

internal class GetAllBrandModelQueryHandler : IRequestHandler<GetAllBrandModelQuery, Fin<List<BrandModelsResponse>>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetAllBrandModelQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<List<BrandModelsResponse>>> Handle(GetAllBrandModelQuery request, CancellationToken cancellationToken)
    {
        return await (
            from brandModel in _dbContext.BrandModels
            select new BrandModelsResponse(
                brandModel.Id.Value,
                brandModel.BrandName,
                brandModel.ModelName,
                brandModel.TransportType,
                brandModel.FuelType,
                brandModel.SeatsNumber,
                brandModel.MaximumLoadCapacityInKillograms,
                _dbContext.Vehicles.AsNoTracking()
                    .Where(v => v.BrandModelId == brandModel.Id && _currentUser.EnterpriseIds.Contains(v.EnterpriseId))
                    .Select(v => v.Id.Value)
                    .ToArray()
            )
        ).ToListAsync(cancellationToken);
    }
}
