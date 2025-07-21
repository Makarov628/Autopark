using MediatR;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.Driver.Entities;
using Autopark.Domain.User.Entities;
using LanguageExt;
using Autopark.UseCases.Common.Models;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Common.ValueObjects;

namespace Autopark.UseCases.Driver.Queries.GetAll;

public class GetAllDriversQueryHandler : IRequestHandler<GetAllDriversQuery, Fin<PagedResult<DriversResponse>>>
{
    private readonly AutoparkDbContext _context;

    public GetAllDriversQueryHandler(AutoparkDbContext context)
    {
        _context = context;
    }

    public async Task<Fin<PagedResult<DriversResponse>>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Drivers
            .Include(d => d.User)
            .Include(d => d.Enterprise)
            .Include(d => d.Vehicle)
            .AsQueryable();

        var enterpriseId = request.EnterpriseId.HasValue
            ? EnterpriseId.Create(request.EnterpriseId.Value)
            : null;
        // var searchString = CyrillicString.Create(request.Search ?? string.Empty);

        // Фильтрация
        if (enterpriseId is not null)
            query = query.Where(d => d.EnterpriseId == enterpriseId);
        // if (searchString.IsSucc)
        //     query = query.Where(d => d.User.FirstName.Value.Contains(request.Search) || d.User.LastName.Value.Contains(request.Search));

        // Сортировка
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortBy = request.SortBy.ToLower();
            var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
            query = (sortBy, sortDirection) switch
            {
                ("lastname", "asc") => query.OrderBy(d => d.User.LastName),
                ("lastname", "desc") => query.OrderByDescending(d => d.User.LastName),
                ("firstname", "asc") => query.OrderBy(d => d.User.FirstName),
                ("firstname", "desc") => query.OrderByDescending(d => d.User.FirstName),
                ("salary", "asc") => query.OrderBy(d => d.Salary),
                ("salary", "desc") => query.OrderByDescending(d => d.Salary),
                _ => query.OrderBy(d => d.Id)
            };
        }
        else
        {
            query = query.OrderBy(d => d.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (request.Page - 1) * request.PageSize;
        var pageItems = await query.Skip(skip).Take(request.PageSize).ToListAsync(cancellationToken);

        var items = pageItems.Select(driver => new DriversResponse(
            driver.Id.Value,
            driver.UserId.Value,
            driver.User.FirstName.Value,
            driver.User.LastName.Value,
            driver.User.DateOfBirth ?? DateTime.MinValue,
            driver.Salary,
            driver.EnterpriseId.Value,
            driver.VehicleId?.Value
        )).ToList();

        return new PagedResult<DriversResponse>(items, request.Page, request.PageSize, totalCount);
    }
}