using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Autopark.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Diagnostics;

namespace Autopark.Web.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, AutoparkDbContext dbContext)
    {
        // Получаем токен из Authorization header
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        // Если токен не найден в header, пробуем получить из cookie
        if (string.IsNullOrEmpty(token))
        {
            token = context.Request.Cookies["access_token"];
            Debug.WriteLine($"Токен найден в cookie: {!string.IsNullOrEmpty(token)}");
        }
        else
        {
            Debug.WriteLine("Токен найден в Authorization header");
        }

        if (token != null)
        {
            try
            {
                Debug.WriteLine("Начинаем валидацию JWT токена");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "your-secret-key-here");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                Debug.WriteLine($"Токен валиден, UserId: {userId}");

                // Получаем пользователя из базы данных
                var user = await dbContext.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == UserId.Create(userId));

                if (user == null)
                {
                    Debug.WriteLine("Пользователь не найден в базе данных");
                    await _next(context);
                    return;
                }

                Debug.WriteLine($"Пользователь найден: {user.Email}");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                // Добавляем роли пользователя
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Role.ToString()));
                    Debug.WriteLine($"Добавлена роль: {role.Role}");
                }

                // Получаем enterprise IDs для менеджера
                var enterpriseIds = user.Roles.Any(r => r.Role == UserRoleType.Admin)
                    ? await dbContext.Enterprises.Select(e => e.Id).ToListAsync()
                    : await dbContext.ManagerEnterprises
                                .Where(me => me.Manager.UserId == user.Id)
                                .Select(me => me.EnterpriseId)
                                .ToListAsync();

                foreach (var enterpriseId in enterpriseIds)
                {
                    claims.Add(new Claim("enterprise", enterpriseId.Value.ToString()));
                    Debug.WriteLine($"Добавлено предприятие: {enterpriseId.Value}");
                }

                var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
                context.User = new ClaimsPrincipal(identity);
                Debug.WriteLine("ClaimsPrincipal создан успешно");

            }
            catch (Exception ex)
            {
                // Токен недействителен, но продолжаем выполнение
                Debug.WriteLine($"Ошибка валидации токена: {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine("Токен не найден");
        }

        await _next(context);
    }
}