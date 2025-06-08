using System.Security.Claims;
using Autopark.Infrastructure.Database;
using Autopark.Infrastructure.Database.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Autopark.Web.Controllers;

public record SetAdminPasswordDto(string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    public AuthController()
    {
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<User?>> GetMe([FromServices] AutoparkDbContext context)
    {
        var claims = HttpContext.User.Claims;
        if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            return Unauthorized("User ID claim not found.");

        string userId = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        return await context.Users.FindAsync(userId);
    }

    [HttpPost("setup-admin")]
    [AllowAnonymous]
    public async Task<ActionResult> SetupAdmin([FromBody] SetAdminPasswordDto dto,
        [FromServices] IConfiguration cfg,
        [FromServices] UserManager<User> users)
    {
        var email = cfg["Admin:Email"] ?? "admin@example.com";
        var admin = await users.FindByEmailAsync(email);
        if (admin is null) return NotFound();
        if (admin.IsPasswordInitialized) return Forbid();

        var token = await users.GeneratePasswordResetTokenAsync(admin);
        var result = await users.ResetPasswordAsync(admin, token, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        admin.IsPasswordInitialized = true;
        await users.UpdateAsync(admin);

        return Ok("Пароль сохранён. Эндпоинт больше не доступен.");
    }

}