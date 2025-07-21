using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using Autopark.UseCases.User.Commands.Login;
using Autopark.Infrastructure.Database.Services;
using Autopark.UseCases.Common.Exceptions;
using System.Diagnostics;

namespace Autopark.Web.Pages.Manager;

public class LoginModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginModel(IMediator mediator, IPasswordHasher passwordHasher)
    {
        _mediator = mediator;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
    public bool EmailError { get; set; }
    public bool PasswordError { get; set; }

    public void OnGet()
    {
        // Если пользователь уже авторизован (есть JWT токен), перенаправляем в кабинет
        if (User.Identity?.IsAuthenticated == true)
        {
            Response.Redirect("/Manager");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Debug.WriteLine($"OnPostAsync вызван. Email: {Email}, Password: {Password}");

        // Сброс ошибок
        EmailError = false;
        PasswordError = false;
        ErrorMessage = string.Empty;

        // Валидация
        if (string.IsNullOrWhiteSpace(Email))
        {
            EmailError = true;
            ErrorMessage = "Пожалуйста, введите email.";
            Debug.WriteLine("Ошибка: Email пустой");
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = true;
            ErrorMessage = "Пожалуйста, введите пароль.";
            Debug.WriteLine("Ошибка: Password пустой");
            return Page();
        }

        try
        {
            Debug.WriteLine($"Создаем LoginUserCommand для email: {Email}");

            // Создаем команду для входа
            var loginCommand = new LoginUserCommand
            {
                Email = Email,
                Password = Password,
                DeviceName = "Manager Web Interface",
                DeviceType = Autopark.Domain.User.Entities.DeviceType.Web
            };

            Debug.WriteLine("Отправляем команду через MediatR");
            var loginResponse = await _mediator.Send(loginCommand);
            Debug.WriteLine($"Получен ответ: UserId={loginResponse.UserId}, Email={loginResponse.Email}");

            if (!loginResponse.Roles.Contains(Domain.User.Entities.UserRoleType.Manager))
            {
                ErrorMessage = "Вы не являетесь менеджером. Вход воспрещён.";
                return Page();
            }

            // Сохраняем JWT токен в cookie для последующего использования
            Response.Cookies.Append("access_token", loginResponse.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // В продакшене должно быть true
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(8)
            });

            Debug.WriteLine("JWT токен сохранен в cookie, перенаправляем в кабинет");
            // Перенаправляем в кабинет менеджера
            return RedirectToPage("/Manager/Index");
        }
        catch (UnauthorizedException ex)
        {
            Debug.WriteLine($"UnauthorizedException: {ex.Message}");
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Общая ошибка: {ex.Message}");
            Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            ErrorMessage = "Произошла ошибка при входе. Попробуйте еще раз.";
            return Page();
        }
    }
}