
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Enterprise.Entities;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace Autopark.UseCases.Enterprise.Commands.Create;

internal class CreateEnterpriseCommandHandler : IRequestHandler<CreateEnterpriseCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateEnterpriseCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(CreateEnterpriseCommand request, CancellationToken cancellationToken)
    {
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

        var enterprise = EnterpriseEntity.Create(
            name: name.Head(),
            address: address
        );

        await _dbContext.Enterprises.AddAsync(enterprise, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return LanguageExt.Unit.Default;
    }
}