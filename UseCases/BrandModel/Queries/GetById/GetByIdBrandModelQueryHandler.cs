using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Domain.BrandModel.ValueObjects;
using LanguageExt.Common;
using Autopark.Infrastructure.Database.Identity;

namespace Autopark.UseCases.BrandModel.Queries.GetById;

internal class GetByIdBrandModelQueryHandler : IRequestHandler<GetByIdBrandModelQuery, Fin<BrandModelResponse>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetByIdBrandModelQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<BrandModelResponse>> Handle(GetByIdBrandModelQuery request, CancellationToken cancellationToken)
    {
        var brandModelId = BrandModelId.Create(request.Id);
        var brandModel = await _dbContext.BrandModels
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == brandModelId, cancellationToken);

        if (brandModel is null)
            return Error.New($"Brand model not found with id: {request.Id}");

        var vehiclesIds = await _dbContext.Vehicles
            .AsNoTracking()
            .Where(v => v.BrandModelId == brandModelId && _currentUser.EnterpriseIds.Contains(v.EnterpriseId))
            .Select(v => v.Id)
            .ToListAsync(cancellationToken);

        return new BrandModelResponse(
            brandModel.Id.Value,
            brandModel.BrandName,
            brandModel.ModelName,
            brandModel.TransportType,
            brandModel.FuelType,
            brandModel.SeatsNumber,
            brandModel.MaximumLoadCapacityInKillograms,
            vehiclesIds.Select(v => v.Value).ToArray()
        );
    }
}
