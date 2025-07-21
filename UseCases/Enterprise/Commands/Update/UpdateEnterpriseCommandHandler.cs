
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.Enterprise.Commands.Update;

internal class UpdateEnterpriseCommandHandler : IRequestHandler<UpdateEnterpriseCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public UpdateEnterpriseCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(UpdateEnterpriseCommand request, CancellationToken cancellationToken)
    {
        var enterpriseId = EnterpriseId.Create(request.Id);
        var enterprise = await _dbContext.Enterprises.AsNoTracking().FirstOrDefaultAsync(v => v.Id == enterpriseId, cancellationToken);
        if (enterprise is null)
            return Error.New($"Enterprise not found with id: {request.Id}");

        var address = request.Address.Trim();
        if (string.IsNullOrWhiteSpace(address))
            return Error.New("Адрес предприятия не может быть пустым");

        var name = CyrillicString.Create(request.Name);
        var potentialErrors = new Either<Error, ValueObject>[]
        {
            name.ToValueObjectEither()
        };

        var aggregatedErrorMessage = potentialErrors
            .MapLeftT(error => error.Message)
            .Lefts()
            .JoinStrings("; ");

        if (!aggregatedErrorMessage.IsNullOrEmpty())
            return Error.New(aggregatedErrorMessage);

        enterprise.Update(
            name: name.Head(),
            address: address,
            timeZoneId: request.TimeZone
        );

        _dbContext.Enterprises.Attach(enterprise);
        _dbContext.Entry(enterprise).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return LanguageExt.Unit.Default;
    }
}