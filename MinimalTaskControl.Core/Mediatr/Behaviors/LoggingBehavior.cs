using MediatR;
using Microsoft.Extensions.Logging;
using MinimalTaskControl.Core.Exceptions;
using System.Diagnostics;

namespace MinimalTaskControl.Core.Mediatr.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["RequestId"] = requestId,
            ["RequestName"] = requestName,
            ["RequestType"] = typeof(TRequest).FullName!
        }))
        {
            try
            {
                logger.LogInformation("Начало обработки запроса");

                var stopwatch = Stopwatch.StartNew();
                var response = await next(cancellationToken);
                stopwatch.Stop();

                logger.LogInformation("Запрос обработан успешно за {ElapsedMs} мс",
                    stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (BaseException ex)
            {
                switch (ex)
                {
                    case ValidationException:
                        logger.LogWarning(ex, "Ошибка валидации: {LogString}", ex.ToLogString());
                        break;
                    case NotFoundException:
                        logger.LogWarning(ex, "Объект не найден: {LogString}", ex.ToLogString());
                        break;
                    case BusinessException:
                        logger.LogWarning(ex, "Ошибка бизнес-логики: {LogString}", ex.ToLogString());
                        break;
                    case AccessDeniedException:
                        logger.LogWarning(ex, "Доступ запрещен: {LogString}", ex.ToLogString());
                        break;
                    default:
                        logger.LogWarning(ex, "Бизнес-ошибка: {LogString}", ex.ToLogString());
                        break;
                }
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Критическая ошибка: {Message}", ex.ToString());
                throw;
            }
        }
    }
}
