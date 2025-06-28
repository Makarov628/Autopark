using Autopark.Domain.User.Entities;
using Autopark.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autopark.UseCases.User.Queries.GetActivationToken;

public class GetActivationTokenQueryHandler : IRequestHandler<GetActivationTokenQuery, GetActivationTokenResponse>
{
    private readonly AutoparkDbContext _context;

    public GetActivationTokenQueryHandler(AutoparkDbContext context)
    {
        _context = context;
    }

    public async Task<GetActivationTokenResponse> Handle(GetActivationTokenQuery request, CancellationToken cancellationToken)
    {
        var activationToken = await _context.ActivationTokens
            .Include(at => at.User)
            .FirstOrDefaultAsync(at => at.User.Email == request.Email, cancellationToken);

        if (activationToken == null)
        {
            return new GetActivationTokenResponse { Success = false };
        }

        return new GetActivationTokenResponse
        {
            Token = activationToken.Token,
            Success = true
        };
    }
}