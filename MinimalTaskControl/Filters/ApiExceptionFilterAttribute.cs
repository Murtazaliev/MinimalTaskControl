using Microsoft.AspNetCore.Mvc.Filters;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.WebApi.Extensions;

namespace MinimalTaskControl.WebApi.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is BaseException)
        {
            var apiResult = context.Exception.ToResponse<object>();
            context.Result = apiResult.ToHttpResponse();
            context.ExceptionHandled = true;
        }
        else
        {
            var apiResult = context.Exception.ToResponse<object>();
            context.Result = apiResult.ToHttpResponse();
            context.ExceptionHandled = true;
        }
    }
}
