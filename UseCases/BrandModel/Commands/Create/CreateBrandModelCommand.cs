using MediatR;
using LanguageExt;
using Unit = LanguageExt.Unit;
using Autopark.Domain.BrandModel.Enums;

namespace Autopark.UseCases.BrandModel.Commands.Create;

public record CreateBrandModelCommand(
        string BrandName,
        string ModelName,
        TransportType TransportType,
        FuelType FuelType,
        uint SeatsNumber,
        uint MaximumLoadCapacityInKillograms
) : IRequest<Fin<Unit>>;