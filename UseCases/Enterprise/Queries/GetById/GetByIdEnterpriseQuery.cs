using Autopark.Domain.BrandModel.Enums;
using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Enterprise.Queries.GetById;

public record GetByIdEnterpriseQuery(int Id) : IRequest<Fin<EnterpriseResponse>>;

public record EnterpriseResponse(
    int Id,
    string Name,
    string Address,
    int[] VehicleIds,
    int[] DriverIds
);