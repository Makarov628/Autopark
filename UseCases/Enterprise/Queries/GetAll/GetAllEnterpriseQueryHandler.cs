using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database.Identity;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.Enterprise.Queries.GetAll;

internal class GetAllEnterprisesQueryHandler : IRequestHandler<GetAllEnterprisesQuery, Fin<PagedResult<EnterprisesResponse>>>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ICurrentUser _currentUser;

    public GetAllEnterprisesQueryHandler(AutoparkDbContext dbContext, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Fin<PagedResult<EnterprisesResponse>>> Handle(GetAllEnterprisesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Enterprises
            .Where(e => _currentUser.EnterpriseIds.Contains(e.Id));

        // Фильтрация
        // if (!string.IsNullOrEmpty(request.Search))
        //     query = query.Where(e => e.Name.Value.Contains(request.Search) || e.Address.Contains(request.Search));

        // Сортировка
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var sortBy = request.SortBy.ToLower();
            var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
            query = (sortBy, sortDirection) switch
            {
                ("name", "asc") => query.OrderBy(e => e.Name),
                ("name", "desc") => query.OrderByDescending(e => e.Name),
                ("address", "asc") => query.OrderBy(e => e.Address),
                ("address", "desc") => query.OrderByDescending(e => e.Address),
                _ => query.OrderBy(e => e.Id)
            };
        }
        else
        {
            query = query.OrderBy(e => e.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (request.Page - 1) * request.PageSize;
        var pageItems = await query.Skip(skip).Take(request.PageSize).ToListAsync(cancellationToken);

        var items = pageItems.Select(enterprise => new EnterprisesResponse(
            enterprise.Id.Value,
            enterprise.Name.Value,
            enterprise.Address,
            enterprise.TimeZoneId,
            _dbContext.Vehicles.AsNoTracking()
                .Where(v => v.EnterpriseId == enterprise.Id)
                .Select(v => v.Id.Value)
                .ToArray(),
            _dbContext.Drivers.AsNoTracking()
                .Where(d => d.EnterpriseId == enterprise.Id)
                .Select(d => d.Id.Value)
                .ToArray(),
            _dbContext.ManagerEnterprises.AsNoTracking()
                .Where(me => me.EnterpriseId == enterprise.Id)
                .Select(me => me.ManagerId.Value.ToString())
                .ToArray()
        )).ToList();

        return new PagedResult<EnterprisesResponse>(items, request.Page, request.PageSize, totalCount);
    }
}