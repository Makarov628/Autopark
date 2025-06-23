using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;

namespace Autopark.UseCases.User.Queries.GetById;

internal class GetByIdUserQueryHandler : IRequestHandler<GetByIdUserQuery, Fin<UserResponse>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetByIdUserQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<UserResponse>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == UserId.Create(request.Id), cancellationToken);

            if (user is null)
                return Error.New($"User not found with id: {request.Id}");

            var response = new UserResponse(
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
            );

            return response;
        }
        catch (Exception ex)
        {
            return Error.New($"Failed to get user: {ex.Message}");
        }
    }
}