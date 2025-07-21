using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database.Identity;
using Autopark.UseCases.Common.Models;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.BrandModel.ValueObjects;
using Autopark.Infrastructure.Database.Services;

namespace Autopark.UseCases.Vehicle.Queries.GetAll;

internal class GetAllVehicleQueryHandler : IRequestHandler<GetAllVehiclesQuery, Fin<PagedResult<VehiclesResponse>>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ITimeZoneService _timeZoneService;

    public GetAllVehicleQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser, ITimeZoneService timeZoneService)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _timeZoneService = timeZoneService;
    }

    public async Task<Fin<PagedResult<VehiclesResponse>>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Vehicles
            .Include(v => v.Enterprise)
            .Where(vehicle => _currentUser.EnterpriseIds.Contains(vehicle.EnterpriseId));

        var enterpriseId = request.EnterpriseId.HasValue
            ? EnterpriseId.Create(request.EnterpriseId.Value)
            : null;
        var brandModelId = request.BrandModelId.HasValue
            ? BrandModelId.Create(request.BrandModelId.Value)
            : null;

        // Фильтрация
        if (enterpriseId is not null)
            query = query.Where(v => v.EnterpriseId == enterpriseId);
        if (brandModelId is not null)
            query = query.Where(v => v.BrandModelId == brandModelId);
        // if (!string.IsNullOrEmpty(request.Search))
        //     query = query.Where(v => v.Name.Value.Contains(request.Search) || v.RegistrationNumber.Value.Contains(request.Search));

        // Сортировка
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortBy = request.SortBy.ToLower();
            var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
            query = (sortBy, sortDirection) switch
            {
                ("name", "asc") => query.OrderBy(v => v.Name),
                ("name", "desc") => query.OrderByDescending(v => v.Name),
                ("registrationnumber", "asc") => query.OrderBy(v => v.RegistrationNumber),
                ("registrationnumber", "desc") => query.OrderByDescending(v => v.RegistrationNumber),
                ("price", "asc") => query.OrderBy(v => v.Price),
                ("price", "desc") => query.OrderByDescending(v => v.Price),
                ("purchasedate", "asc") => query.OrderBy(v => v.PurchaseDate),
                ("purchasedate", "desc") => query.OrderByDescending(v => v.PurchaseDate),
                _ => query.OrderBy(v => v.Id)
            };
        }
        else
        {
            query = query.OrderBy(v => v.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (request.Page - 1) * request.PageSize;
        var pageItemsQuery = query.Skip(skip).Take(request.PageSize);

        var items = await (
            from vehicle in pageItemsQuery
            let enterpriseTimeZoneId = vehicle.Enterprise.TimeZoneId
            let purchaseDateInEnterpriseTime = vehicle.PurchaseDate.HasValue
                ? _timeZoneService.ToEnterpriseZone(vehicle.PurchaseDate.Value, enterpriseTimeZoneId)
                : (DateTimeOffset?)null
            select new VehiclesResponse(
                vehicle.Id.Value,
                vehicle.Name.Value,
                vehicle.Price.Value,
                vehicle.MileageInKilometers.ValueInKilometers,
                vehicle.Color.Value,
                vehicle.RegistrationNumber.Value,
                vehicle.BrandModelId.Value,
                vehicle.EnterpriseId.Value,
                vehicle.ActiveDriverId != null ? vehicle.ActiveDriverId.Value : null,
                purchaseDateInEnterpriseTime,
                _dbContext.Drivers.AsNoTracking()
                    .Where(d => d.EnterpriseId == vehicle.EnterpriseId && d.VehicleId == vehicle.Id)
                    .Select(d => d.Id.Value)
                    .ToArray()
            )
        ).ToListAsync(cancellationToken);

        return new PagedResult<VehiclesResponse>(items, request.Page, request.PageSize, totalCount);
    }
}
