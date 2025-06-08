

using Microsoft.AspNetCore.Identity;

namespace Autopark.Infrastructure.Database.Identity;

public class User : IdentityUser
{
    public bool IsPasswordInitialized { get; set; } = true;
}