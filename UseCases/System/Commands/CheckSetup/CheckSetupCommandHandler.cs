using MediatR;
using Microsoft.EntityFrameworkCore;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;

namespace Autopark.UseCases.System.Commands.CheckSetup;

public class CheckSetupCommandHandler : IRequestHandler<CheckSetupCommand, CheckSetupResponse>
{
    private readonly AutoparkDbContext _context;

    public CheckSetupCommandHandler(AutoparkDbContext context)
    {
        _context = context;
    }

    public async Task<CheckSetupResponse> Handle(CheckSetupCommand request, CancellationToken cancellationToken)
    {
        // Проверяем, есть ли пользователи с ролью Admin
        var hasAdmin = await _context.UserRoles
            .AnyAsync(r => r.Role == UserRoleType.Admin, cancellationToken);

        return new CheckSetupResponse(hasAdmin);
    }
}