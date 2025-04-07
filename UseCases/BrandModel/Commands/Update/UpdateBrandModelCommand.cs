using MediatR;
using LanguageExt;
using Autopark.Domain.BrandModel.Enums;

namespace Autopark.UseCases.BrandModel.Commands.Update;

public record UpdateBrandModelCommand(
    int Id,
    string BrandName,
    string ModelName,
    TransportType TransportType,
    FuelType FuelType,
    uint SeatsNumber,
    uint MaximumLoadCapacityInKillograms
) : IRequest<Fin<LanguageExt.Unit>>;