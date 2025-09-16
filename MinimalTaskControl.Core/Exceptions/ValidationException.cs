using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Exceptions;

public class ValidationException : BaseException
{
    public ValidationException(string message,
        Dictionary<string, string>? parametrs = null)
        : base(CoreErrorType.Validation, message,
              parametrs)
    {
    }
}
