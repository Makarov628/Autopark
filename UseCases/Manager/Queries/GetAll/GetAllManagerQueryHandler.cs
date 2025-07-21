using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Manager.ValueObjects;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.Manager.Queries.GetAll;

internal class GetAllManagerQueryHandler : IRequestHandler<GetAllManagerQuery, Fin<PagedResult<ManagersResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllManagerQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<PagedResult<ManagersResponse>>> Handle(GetAllManagerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var managersQuery = _dbContext.Managers
                .Include(m => m.User)
                .Include(m => m.EnterpriseManagers)
                .ThenInclude(em => em.Enterprise)
                .AsNoTracking()
                .AsQueryable();

            // Сортировка
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortBy = request.SortBy.ToLower();
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
                managersQuery = (sortBy, sortDirection) switch
                {
                    ("email", "asc") => managersQuery.OrderBy(m => m.User.Email),
                    ("email", "desc") => managersQuery.OrderByDescending(m => m.User.Email),
                    ("firstname", "asc") => managersQuery.OrderBy(m => m.User.FirstName),
                    ("firstname", "desc") => managersQuery.OrderByDescending(m => m.User.FirstName),
                    ("lastname", "asc") => managersQuery.OrderBy(m => m.User.LastName),
                    ("lastname", "desc") => managersQuery.OrderByDescending(m => m.User.LastName),
                    _ => managersQuery.OrderBy(m => m.Id)
                };
            }
            else
            {
                managersQuery = managersQuery.OrderBy(m => m.Id);
            }

            // Фильтрация
            if (!string.IsNullOrEmpty(request.Search))
            {
                // var search = request.Search.ToLower();
                // managersQuery = managersQuery.Where(m =>
                //     m.User.FirstName.Value.ToLower().Contains(search) ||
                //     m.User.LastName.Value.ToLower().Contains(search) ||
                //     m.User.Email.ToLower().Contains(search)
                // );
            }

            var totalCount = await managersQuery.CountAsync(cancellationToken);
            var skip = (request.Page - 1) * request.PageSize;
            var pageItems = await managersQuery.Skip(skip).Take(request.PageSize).ToListAsync(cancellationToken);

            var response = pageItems.Select(manager => new ManagersResponse(
                Id: manager.Id.Value,
                UserId: manager.UserId.Value,
                Email: manager.User.Email,
                Phone: manager.User.Phone ?? string.Empty,
                FirstName: manager.User.FirstName.Value,
                LastName: manager.User.LastName.Value,
                DateOfBirth: manager.User.DateOfBirth ?? DateTime.MinValue,
                IsActive: manager.User.IsActive,
                EnterpriseIds: manager.EnterpriseManagers.Select(em => em.Enterprise.Id.Value).ToList(),
                CreatedAt: manager.CreatedAt,
                UpdatedAt: manager.UpdatedAt
            )).ToList();

            return new PagedResult<ManagersResponse>(response, request.Page, request.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            return Error.New($"Failed to get managers: {ex.Message}");
        }
    }
}