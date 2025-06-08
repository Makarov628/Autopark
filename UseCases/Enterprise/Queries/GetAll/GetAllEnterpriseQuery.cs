using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Enterprise.Queries.GetAll;

public record GetAllEnterprisesQuery() : IRequest<Fin<List<EnterprisesResponse>>>;

public record EnterprisesResponse(
    int Id,
    string Name,
    string Address,
    int[] VehicleIds,
    int[] DriverIds
);