using Microsoft.AspNetCore.Mvc;
using MinimalTaskControl.WebApi.Models;

namespace MinimalTaskControl.WebApi.Controllers
{
    [ApiController]
    [ProducesResponseType(typeof(ApiResultNoData), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResultNoData), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResultNoData), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResultNoData), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResultNoData), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResultNoData), StatusCodes.Status500InternalServerError)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseController : ControllerBase
    {
    }
}
