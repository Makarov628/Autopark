using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Driver.ValueObjects;

namespace Autopark.UseCases.Driver.Commands.Delete;

internal class DeleteDriverCommandHandler : IRequestHandler<DeleteDriverCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteDriverCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _dbContext.Drivers.AsNoTracking().FirstOrDefaultAsync(v => v.Id == DriverId.Create(request.Id), cancellationToken);
        if (driver is null)
            return Error.New($"Driver not found with id: {request.Id}");

        if (_dbContext.Entry(driver).State == EntityState.Detached)
            _dbContext.Drivers.Attach(driver);

        _dbContext.Drivers.Remove(driver);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}