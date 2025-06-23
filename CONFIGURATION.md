# Конфигурация Autopark

## Обзор настроек

Проект использует несколько файлов конфигурации для разных сред:

- `appsettings.json` - базовые настройки
- `appsettings.Development.json` - настройки для разработки
- `appsettings.Production.json` - настройки для production

## Основные секции конфигурации

### 1. ConnectionStrings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Initial Catalog=Autopark;User Id=sa;Password=your-password;TrustServerCertificate=True"
  }
}
```

### 2. JWT настройки

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key-with-at-least-32-characters",
    "Issuer": "Autopark",
    "Audience": "AutoparkUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

**Важно**: В production используйте секретный ключ длиной не менее 64 символов!

### 3. Администратор

```json
{
  "Admin": {
    "Email": "admin@autopark.com",
    "FirstName": "Админ",
    "LastName": "Админов"
  }
}
```

### 4. Email настройки

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromEmail": "noreply@autopark.com",
    "FromName": "Autopark System"
  }
}
```

### 5. Внешние сервисы

```json
{
  "Messenger": {
    "ApiKey": "your-messenger-api-key",
    "DefaultPhoneNumber": "+79001234567"
  },
  "PushNotifications": {
    "ApiKey": "your-push-notification-api-key",
    "DefaultChannel": "autopark-notifications"
  },
  "Captcha": {
    "ApiKey": "your-captcha-api-key",
    "SecretKey": "your-captcha-secret-key"
  }
}
```

## Настройка для разработки

1. Скопируйте `appsettings.Development.json`
2. Обновите строку подключения к базе данных
3. Установите временные значения для внешних сервисов
4. Используйте короткие сроки жизни токенов для удобства тестирования

## Настройка для production

1. **Безопасность**:

   - Используйте сильный секретный ключ для JWT (64+ символов)
   - Храните секреты в Azure Key Vault или AWS Secrets Manager
   - Используйте переменные окружения для чувствительных данных

2. **База данных**:

   - Используйте production сервер
   - Настройте правильные права доступа
   - Включите SSL/TLS

3. **Email**:

   - Настройте production SMTP сервер
   - Используйте аутентификацию
   - Настройте SPF/DKIM записи

4. **Логирование**:
   - Установите уровень Warning для production
   - Настройте централизованное логирование
   - Мониторинг ошибок

## Переменные окружения

Для production рекомендуется использовать переменные окружения:

```bash
# JWT
JWT__SECRETKEY=your-production-secret-key
JWT__ISSUER=Autopark-Production
JWT__AUDIENCE=AutoparkUsers-Production

# Database
CONNECTIONSTRINGS__DEFAULTCONNECTION=your-production-connection-string

# Email
EMAIL__SMTPPASSWORD=your-email-password

# External services
MESSENGER__APIKEY=your-messenger-api-key
PUSHNOTIFICATIONS__APIKEY=your-push-api-key
CAPTCHA__SECRETKEY=your-captcha-secret
```

## Проверка конфигурации

После настройки проверьте:

1. Подключение к базе данных
2. Генерацию JWT токенов
3. Отправку email (если настроено)
4. Работу внешних сервисов
5. Создание администратора при первом запуске
