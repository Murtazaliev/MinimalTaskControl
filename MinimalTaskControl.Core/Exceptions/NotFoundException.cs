using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Exceptions;

public class NotFoundException: BaseException
{
    public NotFoundException(string entityName, object id,
            string? message = null)
            : base(CoreErrorType.NotFound,
                  message ?? $"{entityName} с ID {id} не найден")
    {
    }
    public NotFoundException(string entityName,
            string? message = null)
            : base(CoreErrorType.NotFound,
                  message ?? $"Список {entityName} не найден")
    {
    }
}
