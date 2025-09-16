using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.WebApi.Enums;
using MinimalTaskControl.WebApi.Extensions;
using MinimalTaskControl.WebApi.Models;
using System.Text.Json;

namespace MinimalTaskControl.WebApi;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            _logger.LogWarning(ex, "Business exception: {LogString}", ex.ToLogString());
            await HandleApiExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleApiExceptionAsync(context, ex);
        }
    }

    private static Task HandleApiExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is BaseException baseEx)
        {
            var result = baseEx.ToResponse<object>();
            return WriteResponseAsync(context, result, baseEx.ErrorType.ToApiErrorType());
        }
        else
        {
            var result = exception.ToResponse<object>();
            return WriteResponseAsync(context, result, ErrorType.Exception);
        }
    }

    private static Task WriteResponseAsync(HttpContext context, ApiResult<object> result, ErrorType errorType)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.AccessDenied => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
}
