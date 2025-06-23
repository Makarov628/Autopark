using System;

namespace Autopark.UseCases.Common.Exceptions;

/// <summary>
/// Исключение для 401 Unauthorized - неверные учётные данные
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
    public UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Исключение для 403 Forbidden - нет прав доступа
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
    public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Исключение для 404 Not Found - ресурс не найден
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Исключение для 409 Conflict - конфликт бизнес-логики
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
    public ConflictException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Исключение для 400 Bad Request - ошибки валидации
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, Exception innerException) : base(message, innerException) { }
}