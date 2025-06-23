using MediatR;
using LanguageExt;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using LanguageExt.Common;
using Autopark.Domain.Manager.ValueObjects;

namespace Autopark.UseCases.Manager.Queries.GetById;

internal class GetByIdManagerQueryHandler : IRequestHandler<GetByIdManagerQuery, Fin<ManagerResponse>>
{
    private readonly AutoparkDbContext _dbContext;

    public GetByIdManagerQueryHandler(AutoparkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Fin<ManagerResponse>> Handle(GetByIdManagerQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var manager = await _dbContext.Managers
                .Include(m => m.User)
                .Include(m => m.EnterpriseManagers)
                .ThenInclude(em => em.Enterprise)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == ManagerId.Create(request.Id), cancellationToken);

            if (manager is null)
                return Error.New($"Manager not found with id: {request.Id}");

            var response = new ManagerResponse(
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
            );

            return response;
        }
        catch (Exception ex)
        {
            return Error.New($"Failed to get manager: {ex.Message}");
        }
    }
}