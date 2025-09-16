using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Exceptions;

public class AccessDeniedException : BaseException
{
    public AccessDeniedException(string resource, string? message = null)
        : base(CoreErrorType.AccessDenied,
              message ?? $"Доступ к ресурсу {resource} запрещен")
    {
    }
}
