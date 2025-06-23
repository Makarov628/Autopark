using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.User.Queries.GetAll;

internal class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, Fin<List<UsersResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllUserQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<List<UsersResponse>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
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

            var users = await usersQuery.ToListAsync(cancellationToken);

            var response = users.Select(user => new UsersResponse(
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

            return response;
        }
        catch (Exception ex)
        {
            return Error.New($"Failed to get users: {ex.Message}");
        }
    }
}