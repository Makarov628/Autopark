using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Unit = LanguageExt.Unit;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Enterprise.ValueObjects;

namespace Autopark.UseCases.Enterprise.Commands.Delete;

internal class DeleteEnterpriseCommandHandler : IRequestHandler<DeleteEnterpriseCommand, Fin<Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public DeleteEnterpriseCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<Unit>> Handle(DeleteEnterpriseCommand request, CancellationToken cancellationToken)
    {
        var enterprise = await _dbContext.Enterprises.AsNoTracking().FirstOrDefaultAsync(v => v.Id == EnterpriseId.Create(request.Id), cancellationToken);
        if (enterprise is null)
            return Error.New($"Enterprise not found with id: {request.Id}");

        if (_dbContext.Entry(enterprise).State == EntityState.Detached)
            _dbContext.Enterprises.Attach(enterprise);

        _dbContext.Enterprises.Remove(enterprise);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}