using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.BrandModel.ValueObjects;

namespace Autopark.UseCases.BrandModel.Commands.Delete;

internal class DeleteBrandModelCommandHandler : IRequestHandler<DeleteBrandModelCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteBrandModelCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteBrandModelCommand request, CancellationToken cancellationToken)
    {
        var brandModelId = BrandModelId.Create(request.Id);
        var brandModel = await _dbContext.BrandModels.AsNoTracking().FirstOrDefaultAsync(v => v.Id == brandModelId, cancellationToken);
        if (brandModel is null)
            return Error.New($"Модель бренда машины с идентификатором '{request.Id}' не существует");

        // Проверяем наличие связанных транспортных средств
        var hasVehicles = await _dbContext.Vehicles.AnyAsync(v => v.BrandModelId == brandModelId, cancellationToken);
        if (hasVehicles)
            return Error.New($"Модель бренда с идентификатором '{brandModelId.Value}' не может быть удалена, так как используется в транспортных средствах");

        if (_dbContext.Entry(brandModel).State == EntityState.Detached)
            _dbContext.BrandModels.Attach(brandModel);

        _dbContext.BrandModels.Remove(brandModel);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}