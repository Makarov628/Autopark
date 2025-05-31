using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.BrandModel.ValueObjects;
using LanguageExt.Common;

namespace Autopark.UseCases.BrandModel.Queries.GetById;

internal class GetByIdBrandModelQueryHandler : IRequestHandler<GetByIdBrandModelQuery, Fin<BrandModelResponse>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetByIdBrandModelQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<BrandModelResponse>> Handle(GetByIdBrandModelQuery request, CancellationToken cancellationToken)
    {
        var brandModelId = BrandModelId.Create(request.Id);
        var brandModel = await _dbContext.BrandModels.AsNoTracking().FirstOrDefaultAsync(b => b.Id == brandModelId, cancellationToken);
        if (brandModel is null)
            return Error.New($"Brand model not found with id: {request.Id}");

        return new BrandModelResponse(
            brandModel.Id.Value,
            brandModel.BrandName,
            brandModel.ModelName,
            brandModel.TransportType,
            brandModel.FuelType,
            brandModel.SeatsNumber,
            brandModel.MaximumLoadCapacityInKillograms
        );
    }
}
