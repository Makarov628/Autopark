using MediatR;

namespace Autopark.UseCases.Trip.Queries.GetTrips;

public sealed record GetTripsQuery(
    int VehicleId,
    DateTime From,
    DateTime To,
    string TimeZoneId = "UTC"
) : IRequest<GetTripsResponse>;