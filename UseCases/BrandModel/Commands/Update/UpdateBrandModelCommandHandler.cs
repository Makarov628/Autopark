using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;

namespace Autopark.UseCases.BrandModel.Commands.Update;

internal class UpdateBrandModelCommandHandler : IRequestHandler<UpdateBrandModelCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public UpdateBrandModelCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(UpdateBrandModelCommand request, CancellationToken cancellationToken)
    {
        var brandModel = await _dbContext.BrandModels.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        if (brandModel is null)
            return Error.New($"Vehicle not found with id: {request.Id}");

        brandModel.Update(
            request.BrandName,
            request.ModelName,
            request.TransportType,
            request.FuelType,
            request.SeatsNumber,
            request.MaximumLoadCapacityInKillograms);

        _dbContext.BrandModels.Attach(brandModel);
        _dbContext.Entry(brandModel).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}