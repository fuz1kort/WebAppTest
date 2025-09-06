using System.ComponentModel.DataAnnotations;
using WebAppTest.Core.Exceptions;

namespace WebAppTest.WebApi.Middlewares;

/// <summary>
/// Middleware для централизованной обработки исключений
/// </summary>
public class ExceptionMiddleware(IWebHostEnvironment environment) : IMiddleware
{
    /// <summary>
    /// Перехватывает и обрабатывает все исключения в приложении
    /// </summary>
    /// <param name="context">HTTP контекст</param>
    /// <param name="next">Следующий делегат в pipeline</param>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    /// <summary>
    /// Форматирует и возвращает JSON ответ с информацией об ошибке
    /// </summary>
    /// <param name="context">HTTP контекст</param>
    /// <param name="exception">Перехваченное исключение</param>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, message) = exception switch
        {
            ApplicationBaseException appException => (
                appException.ResponseStatusCode != default ? (int)appException.ResponseStatusCode : 400,
                "Application Error",
                appException.Message
            ),

            ValidationException => (400, "Validation Error", exception.Message),

            _ => (500, "Internal Server Error", environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = title,
            message,
            status = statusCode
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}