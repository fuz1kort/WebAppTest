using System.Net;

namespace WebAppTest.Core.Exceptions;

/// <summary>
/// Базовый класс для исключений приложения с HTTP статус кодом
/// </summary>
public abstract class ApplicationBaseException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    : Exception(message)
{
    /// <summary>
    /// HTTP статус код ответа
    /// </summary>
    public HttpStatusCode ResponseStatusCode { get; } = statusCode;
}