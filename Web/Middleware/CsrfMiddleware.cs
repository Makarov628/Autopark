using Microsoft.AspNetCore.Antiforgery;

namespace Autopark.Web.Middleware;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IAntiforgery _antiforgery;

    public CsrfMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        _next = next;
        _antiforgery = antiforgery;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Проверяем CSRF токен только для небезопасных методов
        if (ShouldValidateCsrf(context.Request))
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context);
            }
            catch
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid CSRF token");
                return;
            }
        }

        await _next(context);
    }

    private static bool ShouldValidateCsrf(HttpRequest request)
    {
        // Проверяем CSRF только для POST, PUT, DELETE, PATCH методов
        var method = request.Method.ToUpper();
        if (method != "POST" && method != "PUT" && method != "DELETE" && method != "PATCH")
            return false;

        // Исключаем стандартные эндпоинты Identity API
        var path = request.Path.Value?.ToLower();
        if (path == null) return true;

        // Исключаем стандартные эндпоинты Identity API
        var excludedPaths = new[]
        {
            "/login",
            "/register",
            "/confirmemail",
            "/resendconfirmation",
            "/forgotpassword",
            "/resetpassword",
            "/manage/changepassword",
            "/manage/setpassword",
            "/manage/deletepersonaldata",
            "/manage/downloadpersonaldata",
            "/manage/enableauthenticator",
            "/manage/disable2fa",
            "/manage/generaterecoverycodes",
            "/manage/resetauthenticator",
            "/manage/twofactorauth",
            "/manage/unusedcodes"
        };

        return !excludedPaths.Any(excludedPath => path.StartsWith(excludedPath));
    }
}

// Extension method для удобного добавления middleware
public static class CsrfMiddlewareExtensions
{
    public static IApplicationBuilder UseCsrfValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CsrfMiddleware>();
    }
}