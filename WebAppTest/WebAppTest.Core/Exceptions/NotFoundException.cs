using System.Net;

namespace WebAppTest.Core.Exceptions;

/// <summary>
/// Исключение для не найденных ресурсов (HTTP 404)
/// </summary>
public class NotFoundException(string message) : ApplicationBaseException(message, HttpStatusCode.NotFound);