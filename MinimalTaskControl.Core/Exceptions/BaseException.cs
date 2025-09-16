using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Exceptions;

public abstract class BaseException : Exception
{
    public CoreErrorType ErrorType { get; }
    public Dictionary<string, string> Params { get; }

    protected BaseException(CoreErrorType errorType, string message,
        Dictionary<string, string>? parametrs = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorType = errorType;
        Params = parametrs ?? new Dictionary<string, string>();
    }
    public virtual string ToLogString()
    {
        return $"ErrorType: {ErrorType}, Message: {Message}, Params: {string.Join("; ", Params)}";
    }
}
