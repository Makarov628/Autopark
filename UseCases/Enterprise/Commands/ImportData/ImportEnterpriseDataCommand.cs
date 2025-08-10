using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Enterprise.Commands.ImportData;

public record ImportEnterpriseDataCommand(
    string Content,
    string Format, // "json" or "csv"
    bool UpdateExisting = false,
    string? GeoapifyApiKey = null
) : IRequest<Fin<ImportEnterpriseDataResponse>>;

public record ImportEnterpriseDataResponse(
    int EnterprisesImported,
    int VehiclesImported,
    int DriversImported,
    int TripsImported,
    int TrackPointsImported,
    string[] Warnings,
    string[] Errors
);