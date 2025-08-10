using MediatR;

namespace Autopark.UseCases.Trip.Queries.GetTripsByDateRange;

public sealed record GetTripsByDateRangeQuery(
    int VehicleId,
    DateTime From,
    DateTime To,
    string TimeZoneId = "UTC"
) : IRequest<GetTripsByDateRangeResponse>;