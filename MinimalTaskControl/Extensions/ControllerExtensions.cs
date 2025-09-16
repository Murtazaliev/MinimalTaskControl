using Microsoft.AspNetCore.Mvc;
using MinimalTaskControl.WebApi.Enums;
using MinimalTaskControl.WebApi.Models;

namespace MinimalTaskControl.WebApi.Extensions;

public static class ControllerExtensions
{
    public static IActionResult ToHttpResponse<TData>(this ApiResult<TData> apiResult)
    {
        return apiResult.Error switch
        {
            ErrorType.None => new OkObjectResult(apiResult), 
            
            ErrorType.AccessDenied => new ObjectResult(apiResult)
            { StatusCode = StatusCodes.Status405MethodNotAllowed },

            ErrorType.Validation => new BadRequestObjectResult(apiResult)
            { StatusCode = StatusCodes.Status400BadRequest},

            ErrorType.Error => new ObjectResult(apiResult)
            { StatusCode = StatusCodes.Status403Forbidden },

            ErrorType.NotFound => new NotFoundObjectResult(apiResult)
            { StatusCode = StatusCodes.Status404NotFound },

            ErrorType.Exception => new ObjectResult(apiResult)
            { StatusCode = StatusCodes.Status500InternalServerError },

            _ => new ObjectResult(apiResult)
            { StatusCode = StatusCodes.Status500InternalServerError }
        };
    }

    public static IActionResult ToHttpResponse<TData>(this object result)
    {
        var apiResult = result.ToResponse<TData>();
        return apiResult.ToHttpResponse();
    }
}
