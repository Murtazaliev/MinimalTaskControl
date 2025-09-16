using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Exceptions;

[Serializable]
public class BusinessException : BaseException
{
    public BusinessException(string message,
            Dictionary<string, string>? parametrs = null,
            Exception? innerException = null)
            : base(CoreErrorType.Error, message, parametrs, innerException)
    {
    }

    public BusinessException(string message, string paramKey, string paramValue)
        : this(message, new Dictionary<string, string> { { paramKey, paramValue } })
    {
    }

    public BusinessException(string message, Exception innerException)
        : this(message, null, innerException)
    {
    }
}