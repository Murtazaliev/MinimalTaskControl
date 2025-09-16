using MediatR;
using Microsoft.Extensions.Logging;
using MinimalTaskControl.Core.Exceptions;
using System;
using System.Diagnostics;

namespace MinimalTaskControl.Core.Mediatr.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["RequestName"] = requestName,
            ["RequestType"] = typeof(TRequest).FullName
        }))
        {
            try
            {
                _logger.LogInformation("Начало обработки запроса");

                var stopwatch = Stopwatch.StartNew();
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("Запрос обработан успешно за {ElapsedMs} мс",
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (BaseException ex)
            {
                switch (ex)
                {
                    case ValidationException:
                        _logger.LogWarning(ex, "Ошибка валидации: {LogString}", ex.ToLogString());
                        break;
                    case NotFoundException:
                        _logger.LogWarning(ex, "Объект не найден: {LogString}", ex.ToLogString());
                        break;
                    case BusinessException:
                        _logger.LogWarning(ex, "Ошибка бизнес-логики: {LogString}", ex.ToLogString());
                        break;
                    case AccessDeniedException:
                        _logger.LogWarning(ex, "Доступ запрещен: {LogString}", ex.ToLogString());
                        break;
                    default:
                        _logger.LogWarning(ex, "Бизнес-ошибка: {LogString}", ex.ToLogString());
                        break;
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка: {Message}", ex.ToString());
                throw;
            }
        }
    }
}
