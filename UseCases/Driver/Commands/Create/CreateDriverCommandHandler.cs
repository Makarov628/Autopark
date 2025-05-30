
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Infrastructure.Database;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.Driver.Commands.Create;

internal class CreateDriverCommandHandler : IRequestHandler<CreateDriverCommand, Fin<LanguageExt.Unit>>
{
    private readonly AutoparkDbContext _dbContext;

    public CreateDriverCommandHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<LanguageExt.Unit>> Handle(CreateDriverCommand request, CancellationToken cancellationToken)
    {
        var enterpriseId = EnterpriseId.Create(request.EnterpriseId);
        var enterpriseExists = await _dbContext.Enterprises.AnyAsync(e => e.Id == enterpriseId, cancellationToken);
        if (!enterpriseExists)
            return Error.New($"Предприятие с идентификатором '{enterpriseId.Value}' не существует");

        var lastName = CyrillicString.Create(request.LastName);
        var firstName = CyrillicString.Create(request.FirstName);

        var potentialErrors = new Either<Error, ValueObject>[]
        {
            lastName.ToValueObjectEither(),
            firstName.ToValueObjectEither()
        };

        var aggregatedErrorMessage = potentialErrors
            .MapLeftT(error => error.Message)
            .Lefts()
            .JoinStrings("; ");

        if (!aggregatedErrorMessage.IsNullOrEmpty())
            return Error.New(aggregatedErrorMessage);

        var driver = DriverEntity.Create(
            firstName: firstName.Head(),
            lastName: lastName.Head(),
            dateOfBirth: request.DateOfBirth,
            salary: request.Salary,
            enterpriseId: enterpriseId
        );

        await _dbContext.Drivers.AddAsync(driver, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return LanguageExt.Unit.Default;
    }
}