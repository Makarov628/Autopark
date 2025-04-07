using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Autopark.Domain.BrandModel.Entities;

namespace Autopark.UseCases.BrandModel.Commands.Create;

internal class CreateBrandModelCommandHandler : IRequestHandler<CreateBrandModelCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateBrandModelCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(CreateBrandModelCommand request, CancellationToken cancellationToken)
    {
        var brandModel = BrandModelEntity.Create(
            request.BrandName,
            request.ModelName,
            request.TransportType,
            request.FuelType,
            request.SeatsNumber,
            request.MaximumLoadCapacityInKillograms);

        await _dbContext.BrandModels.AddAsync(brandModel, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}