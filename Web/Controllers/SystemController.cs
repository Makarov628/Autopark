using Microsoft.AspNetCore.Mvc;
using MediatR;
using Autopark.UseCases.System.Commands.CheckSetup;
using Autopark.UseCases.System.Commands.InitialSetup;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Проверяет статус первоначальной настройки системы
    /// </summary>
    [HttpGet("check-setup")]
    public async Task<IActionResult> CheckSetup()
    {
        try
        {
            var command = new CheckSetupCommand();
            var response = await _mediator.Send(command);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ошибка при проверке настройки системы", error = ex.Message });
        }
    }

    /// <summary>
    /// Выполняет первоначальную настройку системы
    /// </summary>
    [HttpPost("initial-setup")]
    public async Task<IActionResult> InitialSetup([FromBody] InitialSetupRequest request)
    {
        try
        {
            if (request?.AdminData == null)
            {
                return BadRequest(new { message = "Данные администратора обязательны" });
            }

            var command = new InitialSetupCommand(
                request.AdminData.Email,
                request.AdminData.FirstName,
                request.AdminData.LastName,
                request.AdminData.Phone,
                request.AdminData.Password
            );

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ошибка при настройке системы", error = ex.Message });
        }
    }

    /// <summary>
    /// Получает информацию о системе
    /// </summary>
    [HttpGet("info")]
    public IActionResult GetSystemInfo()
    {
        try
        {
            var info = new
            {
                Name = "Autopark Management System",
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                Timestamp = DateTime.UtcNow
            };

            return Ok(info);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ошибка при получении информации о системе", error = ex.Message });
        }
    }

    /// <summary>
    /// Проверяет здоровье системы
    /// </summary>
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        try
        {
            var health = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Uptime = Environment.TickCount64
            };

            return Ok(health);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { status = "Unhealthy", error = ex.Message });
        }
    }
}

// Модели запросов
public class InitialSetupRequest
{
    public AdminData AdminData { get; set; } = null!;
}

public class AdminData
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}