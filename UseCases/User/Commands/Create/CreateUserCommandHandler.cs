using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Autopark.Infrastructure.Database;
using Autopark.Domain.User.Entities;
using Autopark.Domain.User.ValueObjects;
using Autopark.Domain.Common.ValueObjects;
using Autopark.Infrastructure.Database.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace Autopark.UseCases.User.Commands.Create;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly AutoparkDbContext _dbContext;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(
        AutoparkDbContext dbContext,
        ITokenGenerator tokenGenerator,
        IEmailService emailService)
    {
        _dbContext = dbContext;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Проверяем, что email не занят
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser is not null && existingUser.IsActive)
        {
            throw new InvalidOperationException($"Пользователь с email {request.Email} уже существует");
        }

        // 2. Создаём пользователя
        var firstName = CyrillicString.Create(request.FirstName).ThrowIfFail();
        var lastName = CyrillicString.Create(request.LastName).ThrowIfFail();

        var user = existingUser is not null
        ? existingUser
        : new UserEntity
        {
            Email = request.Email,
            Phone = request.Phone,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = request.DateOfBirth,
            IsActive = false, // Пользователь неактивен до активации
            EmailConfirmed = null,
            PhoneConfirmed = null
        };

        if (existingUser is null)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken); // Сохраняем, чтобы получить user.Id
        }

        // 3. Создаём Credentials (без пароля, пользователь задаст при активации)
        var credentials = new Credentials
        {
            UserId = user.Id,
            Type = CredentialsType.Local,
            PasswordHash = null, // Пароль будет задан при активации
            Salt = null,
            ProviderUserId = null,
            ProviderEmail = null
        };
        _dbContext.Credentials.Add(credentials);

        // 4. Создаём токен активации
        var activationToken = new ActivationToken
        {
            UserId = user.Id,
            Token = _tokenGenerator.GenerateActivationToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Токен действителен 7 дней
            Type = ActivationTokenType.Email
        };
        _dbContext.ActivationTokens.Add(activationToken);

        // 5. Сохраняем изменения
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 6. Отправляем письмо с токеном активации
        var activationLink = $"http://localhost:8080/activate?token={activationToken.Token}";
        var emailBody = $@"
            <h2>Добро пожаловать в Autopark!</h2>
            <p>Здравствуйте, {firstName.Value} {lastName.Value}!</p>
            <p>Для активации вашего аккаунта перейдите по ссылке:</p>
            <p><a href='{activationLink}'>{activationLink}</a></p>
            <p>Ссылка действительна до {activationToken.ExpiresAt:dd.MM.yyyy HH:mm} UTC</p>
            <p>Если вы не регистрировались в системе, проигнорируйте это письмо.</p>";

        await _emailService.SendEmailAsync(
            request.Email,
            "Активация аккаунта в Autopark",
            emailBody);

        return user.Id.Value;
    }
}