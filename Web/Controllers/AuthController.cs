using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Autopark.Domain.Common;
using Autopark.Domain.Common.Models;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Domain.Enterprise.ValueObjects;
using Autopark.Domain.Manager.Entities;
using Autopark.Infrastructure.Database;
using Autopark.Infrastructure.Database.Identity;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autopark.Web.Controllers;

public record SetAdminPasswordDto(string Password);

public record RegisterManagerDto(
    string LastName,
    string FirstName,
    [EmailAddress] string Email,
    string Password);

public record UserInfo(
    string Id,
    string Login,
    string Role,
    IReadOnlyList<int> EnterpriseIds
);

public record AttachToEnterprise(
    int enterpriseId
);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ManagerEntity> _signInManager;

    public AuthController(SignInManager<ManagerEntity> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfo>> GetMe([FromServices] ICurrentUser currentUser)
    {
        // var claims = HttpContext.User.Claims;
        // if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
        //     return Unauthorized("User ID claim not found.");

        // string userId = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        // return await context.Users.FindAsync(userId);
        await Task.CompletedTask;
        return Ok(new UserInfo(
            currentUser.Id,
            currentUser.Login,
            currentUser.Role,
            currentUser.EnterpriseIds.Select(e => e.Value).ToArray()));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        // Уничтожаем пользовательскую сессию
        await _signInManager.SignOutAsync();

        // Возвращаем успешный ответ
        return Ok(new { message = "Успешный выход из системы" });

        // Альтернативно можно использовать встроенный эндпоинт Identity API:
        // POST /logout (автоматически создается через MapIdentityApi)
        // Но тогда нужно обновить клиентский код для использования /logout вместо /api/auth/logout
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] RegisterManagerDto registerManagerDto,
    [FromServices] UserManager<ManagerEntity> users)
    {
        var lastName = CyrillicString.Create(registerManagerDto.LastName);
        var firstName = CyrillicString.Create(registerManagerDto.FirstName);

        var potentialErrors = new Either<Error, ValueObject>[]
        {
            lastName.ToValueObjectEither(),
            firstName.ToValueObjectEither()
        };

        var aggregatedErrorMessage = potentialErrors
            .MapLeftT(error => error.Message)
            .Lefts()
            .JoinStrings("; ");

        if (!aggregatedErrorMessage.IsNullOrEmpty())
            return BadRequest(aggregatedErrorMessage);

        var existsUser = await users.FindByEmailAsync(registerManagerDto.Email);
        if (existsUser is not null)
            return BadRequest($"User is already exists with email {registerManagerDto.Email}");

        existsUser = new ManagerEntity
        {
            Id = Guid.NewGuid().ToString(),
            Email = registerManagerDto.Email,
            UserName = registerManagerDto.Email,
            IsPasswordInitialized = false,
            LastName = lastName.Head(),
            FirstName = firstName.Head()
        };

        var randomPassword = Guid.NewGuid() + "aA!";
        await users.CreateAsync(existsUser, randomPassword);

        var token = await users.GeneratePasswordResetTokenAsync(existsUser);
        var result = await users.ResetPasswordAsync(existsUser, token, registerManagerDto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        existsUser.IsPasswordInitialized = true;
        await users.UpdateAsync(existsUser);
        return Created();
    }

    [HttpPost("setup-admin")]
    [AllowAnonymous]
    public async Task<ActionResult> SetupAdmin([FromBody] SetAdminPasswordDto dto,
        [FromServices] IConfiguration cfg,
        [FromServices] UserManager<ManagerEntity> users)
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

    [HttpPost("attach-to-enterprise")]
    [Authorize]
    public async Task<IActionResult> AttachToEnterprise(
        [FromBody] AttachToEnterprise dto,
        [FromServices] ICurrentUser currentUser,
        [FromServices] AutoparkDbContext db)
    {
        var isAttached = await db.ManagerEnterprises.AnyAsync(m =>
            m.EnterpriseId == EnterpriseId.Create(dto.enterpriseId)
            && m.ManagerId == currentUser.Id);

        if (!isAttached)
        {
            await db.ManagerEnterprises.AddAsync(ManagerEnterpriseEntity.Create(currentUser.Id, EnterpriseId.Create(dto.enterpriseId)));
            await db.SaveChangesAsync();
        }

        return Ok();
    }
}