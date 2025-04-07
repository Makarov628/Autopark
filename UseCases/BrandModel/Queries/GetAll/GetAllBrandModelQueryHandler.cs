using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
namespace Autopark.UseCases.BrandModel.Queries.GetAll;

internal class GetAllBrandModelQueryHandler : IRequestHandler<GetAllBrandModelQuery, Fin<List<BrandModelResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllBrandModelQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<List<BrandModelResponse>>> Handle(GetAllBrandModelQuery request, CancellationToken cancellationToken)
    {
        var brandModels = await _dbContext.BrandModels.AsNoTracking().ToListAsync(cancellationToken);
        return brandModels.ConvertAll(brandModel => new BrandModelResponse(
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
