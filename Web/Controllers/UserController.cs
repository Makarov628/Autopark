using Autopark.UseCases.User.Commands.Create;
using Autopark.UseCases.User.Commands.Login;
using Autopark.UseCases.User.Commands.Logout;
using Autopark.UseCases.User.Commands.Activate;
using Autopark.UseCases.User.Commands.RefreshToken;
using Autopark.UseCases.User.Commands.SetPushToken;
using Autopark.UseCases.User.Queries.GetAll;
using Autopark.UseCases.User.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using GetAllUser = Autopark.UseCases.User.Queries.GetAll;
using GetByIdUser = Autopark.UseCases.User.Queries.GetById;
using Autopark.Infrastructure.Database.Identity;
using Autopark.Domain.User.Entities;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediatr;
    private readonly ICurrentUser _currentUser;

    public UserController(IMediator mediatr, ICurrentUser currentUser)
    {
        _mediatr = mediatr;
        _currentUser = currentUser;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult> Register([FromBody] CreateUserCommand createUserCommand)
    {
        try
        {
            var userId = await _mediatr.Send(createUserCommand, HttpContext.RequestAborted);
            return Created();
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 400);
        }
    }

    [HttpPost("activate")]
    [AllowAnonymous]
    public async Task<ActionResult> Activate([FromBody] ActivateUserCommand activateUserCommand)
    {
        try
        {
            await _mediatr.Send(activateUserCommand, HttpContext.RequestAborted);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 400);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserCommand loginUserCommand)
    {
        try
        {
            var response = await _mediatr.Send(loginUserCommand, HttpContext.RequestAborted);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 400);
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout([FromBody] LogoutCommand logoutCommand)
    {
        try
        {
            await _mediatr.Send(logoutCommand, HttpContext.RequestAborted);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 400);
        }
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        try
        {
            var response = await _mediatr.Send(refreshTokenCommand, HttpContext.RequestAborted);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 400);
        }
    }

    [HttpPost("push-token")]
    [Authorize]
    public async Task<ActionResult> SetPushToken([FromBody] SetPushTokenCommand setPushTokenCommand)
    {
        try
        {
            await _mediatr.Send(setPushTokenCommand, HttpContext.RequestAborted);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 400);
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<GetByIdUser.UserResponse>> GetMe()
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
        {
            return Unauthorized();
        }

        var userId = int.Parse(_currentUser.Id);
        var result = await _mediatr.Send(new GetByIdUserQuery(userId), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<GetAllUser.UserResponse>>> GetAll()
    {
        var result = await _mediatr.Send(new GetAllUserQuery(), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<GetByIdUser.UserResponse>> GetById(int id)
    {
        var result = await _mediatr.Send(new GetByIdUserQuery(id), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }

    [HttpGet("available")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<GetAllUser.UserResponse>>> GetAvailable([FromQuery] string notHasRole)
    {
        if (!Enum.TryParse<UserRoleType>(notHasRole, true, out var roleType))
            return BadRequest($"Некорректная роль: {notHasRole}");

        var result = await _mediatr.Send(new GetAllUserQuery(roleType), HttpContext.RequestAborted);
        return result.Match<ActionResult>(
            Ok,
            error => Problem(detail: error.Message, statusCode: 400));
    }
}