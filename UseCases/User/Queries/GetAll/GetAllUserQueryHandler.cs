using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.User.Entities;
using Autopark.UseCases.Common.Models;

namespace Autopark.UseCases.User.Queries.GetAll;

internal class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Fin<PagedResult<UsersResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllUserQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<PagedResult<UsersResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var usersQuery = _dbContext.Users
                .Include(u => u.Roles)
                .AsNoTracking()
                .AsQueryable();

            if (request.NotHasRole.HasValue)
            {
                var role = request.NotHasRole.Value;
                usersQuery = usersQuery.Where(u => !u.Roles.Any(r => r.Role == role));
            }

            // Фильтрация
            if (request.Role.HasValue)
            {
                var role = request.Role.Value;
                usersQuery = usersQuery.Where(u => u.Roles.Any(r => r.Role == role));
            }
            if (!string.IsNullOrEmpty(request.Search))
            {
                // var search = request.Search.ToLower();
                // usersQuery = usersQuery.Where(u =>
                //     u.Email.ToLower().Contains(search) ||
                //     u.FirstName.Value.ToLower().Contains(search) ||
                //     u.LastName.Value.ToLower().Contains(search)
                // );
            }

            // Сортировка
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortBy = request.SortBy.ToLower();
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
                usersQuery = (sortBy, sortDirection) switch
                {
                    ("email", "asc") => usersQuery.OrderBy(u => u.Email),
                    ("email", "desc") => usersQuery.OrderByDescending(u => u.Email),
                    ("firstname", "asc") => usersQuery.OrderBy(u => u.FirstName),
                    ("firstname", "desc") => usersQuery.OrderByDescending(u => u.FirstName),
                    ("lastname", "asc") => usersQuery.OrderBy(u => u.LastName),
                    ("lastname", "desc") => usersQuery.OrderByDescending(u => u.LastName),
                    _ => usersQuery.OrderBy(u => u.Id)
                };
            }
            else
            {
                usersQuery = usersQuery.OrderBy(u => u.Id);
            }

            var totalCount = await usersQuery.CountAsync(cancellationToken);
            var skip = (request.Page - 1) * request.PageSize;
            var pageItems = await usersQuery.Skip(skip).Take(request.PageSize).ToListAsync(cancellationToken);

            var response = pageItems.Select(user => new UsersResponse(
                Id: user.Id.Value,
                Email: user.Email,
                Phone: user.Phone ?? string.Empty,
                FirstName: user.FirstName.Value,
                LastName: user.LastName.Value,
                DateOfBirth: user.DateOfBirth ?? DateTime.MinValue,
                IsActive: user.IsActive,
                Roles: user.Roles.Select(r => r.Role).ToList(),
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt
            )).ToList();

            return new PagedResult<UsersResponse>(response, request.Page, request.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            return Error.New($"Failed to get users: {ex.Message}");
        }
    }
}