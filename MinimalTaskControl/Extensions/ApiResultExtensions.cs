using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.WebApi.Enums;
using MinimalTaskControl.WebApi.Models;

namespace MinimalTaskControl.WebApi.Extensions
{
    public static class ApiResultExtensions
    {
        public static ApiResult<TData> ToResponse<TData>(this object result)
        {
            switch (result)
            {
                case ApiResult<TData> apiResult:
                    return apiResult;

                case BaseException ex:
                    return ex.ToErrorResponse<TData>();

                case Exception ex:
                    return ex.ToErrorResponse<TData>();

                case TData data:
                    return data.ToSuccessResponse();

                case null when default(TData) == null:
                    return SuccessResponse<TData>(default!, "Успешное выполнение");

                default:
                    throw new InvalidOperationException($"Неизвестный тип результата: {result?.GetType()}");
            }
        }

        private static ApiResult<TData> ToSuccessResponse<TData>(this TData data, string? comment = null)
        {
            return new ApiResult<TData>
            {
                Data = data,
                Error = ErrorType.None,
                Params = new Dictionary<string, string>(),
                Comment = comment ?? "Успешное выполнение"
            };
        }

        private static ApiResult<TData> ToErrorResponse<TData>(this BaseException ex)
        {
            return new ApiResult<TData>
            {
                Data = default,
                Error = ex.ErrorType.ToApiErrorType(),
                Params = ex.Params,
                Comment = ex.Message
            };
        }

        private static ApiResult<TData> ToErrorResponse<TData>(this Exception ex)
        {
            var deepestException = ex;
            while (deepestException.InnerException != null)
            {
                deepestException = deepestException.InnerException;
            }

            return new ApiResult<TData>
            {
                Data = default,
                Error = ErrorType.Exception,
                Params = new Dictionary<string, string>(),
                Comment = deepestException.Message
            };
        }

        private static ApiResult<TData> SuccessResponse<TData>(TData data, string comment)
        {
            return new ApiResult<TData>
            {
                Data = data,
                Error = ErrorType.None,
                Params = new Dictionary<string, string>(),
                Comment = comment
            };
        }
    }
}
