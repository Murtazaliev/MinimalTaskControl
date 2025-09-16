using MinimalTaskControl.Core.Enums;
using MinimalTaskControl.WebApi.Enums;

namespace MinimalTaskControl.WebApi.Extensions;

public static class ErrorTypeMappingExtensions
{
    public static ErrorType ToApiErrorType(this CoreErrorType coreErrorType)
    {
        return coreErrorType switch
        {
            CoreErrorType.NotFound => ErrorType.NotFound,
            CoreErrorType.Validation => ErrorType.Validation,
            CoreErrorType.AccessDenied => ErrorType.AccessDenied,
            CoreErrorType.Error => ErrorType.Error,
            CoreErrorType.Exception => ErrorType.Exception,
            CoreErrorType.None => ErrorType.None,
            _ => ErrorType.Error
        };
    }
}
