using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Manager.ValueObjects;

namespace Autopark.UseCases.Manager.Queries.GetAll;

internal class GetAllManagerQueryHandler : IRequestHandler<GetAllManagerQuery, Fin<List<ManagersResponse>>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetAllManagerQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<List<ManagersResponse>>> Handle(GetAllManagerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var managers = await _dbContext.Managers
                .Include(m => m.User)
                .Include(m => m.EnterpriseManagers)
                .ThenInclude(em => em.Enterprise)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var response = managers.Select(manager => new ManagersResponse(
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

            return response;
        }
        catch (Exception ex)
        {
            return Error.New($"Failed to get managers: {ex.Message}");
        }
    }
}