using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;

namespace Autopark.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CsrfController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;

    public CsrfController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    [HttpGet("token")]
    public IActionResult GetToken()
    {
        // Получаем токены
        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        // Возвращаем токен для использования в заголовках
        return Ok(new
        {
            token = tokens.RequestToken,
            headerName = tokens.HeaderName
        });
    }
}