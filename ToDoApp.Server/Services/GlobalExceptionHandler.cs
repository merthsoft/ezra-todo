using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Server.Services;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ToDoNotFoundException => (StatusCodes.Status404NotFound, ToDoNotFoundException.TodoNotFoundError, (string?)null),
            ToDoServiceException => (StatusCodes.Status400BadRequest, exception.Message, (string?)null),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", exception.Message)
        };

        logger.LogError(exception, "Unhandled exception detected while processing the request.");

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true;
    }
}
