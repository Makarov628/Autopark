using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
namespace Autopark.UseCases.BrandModel.Queries.GetAll;

internal class GetAllBrandModelQueryHandler : IRequestHandler<GetAllBrandModelQuery, Fin<List<BrandModelsResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllBrandModelQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<List<BrandModelsResponse>>> Handle(GetAllBrandModelQuery request, CancellationToken cancellationToken)
    {
        var brandModels = await _dbContext.BrandModels.AsNoTracking().ToListAsync(cancellationToken);
        return brandModels.ConvertAll(brandModel => new BrandModelsResponse(
            brandModel.Id.Value,
            brandModel.BrandName,
            brandModel.ModelName,
            brandModel.TransportType,
            brandModel.FuelType,
            brandModel.SeatsNumber,
            brandModel.MaximumLoadCapacityInKillograms
        ));
    }
}
