using LanguageExt;
using MediatR;

namespace Autopark.UseCases.Enterprise.Queries.ExportData;

public record ExportEnterpriseDataQuery(
    int EnterpriseId,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    string Format = "json" // "json" or "csv"
) : IRequest<Fin<ExportEnterpriseDataResponse>>;

public record ExportEnterpriseDataResponse(
    string Content,
    string ContentType,
    string FileName
);