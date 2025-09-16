using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalTaskControl.Core.Mediatr.Commands.CreateTask;
using MinimalTaskControl.Core.Mediatr.Commands.DeleteTask;
using MinimalTaskControl.Core.Mediatr.Commands.UpdateTask;
using MinimalTaskControl.Core.Mediatr.Queries.GetFilteredTasks;
using MinimalTaskControl.Core.Mediatr.Queries.GetTaskById;
using MinimalTaskControl.WebApi.DTOs.Tasks.Requests;
using MinimalTaskControl.WebApi.Extensions;
using MinimalTaskControl.WebApi.Models;

namespace MinimalTaskControl.WebApi.Controllers.V1
{
    [Authorize]
    public class TasksController : BaseV1Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TasksController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<GetTaskByIdResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var command = new GetTaskByIdQuery(id);

            var result = await _mediator.Send(command);

            return result.ToHttpResponse<GetTaskByIdResult>();
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<List<GetFilteredTasksResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilteredTasks([FromQuery] TaskFilterRequest request)
        {
            var command = _mapper.Map<GetFilteredTasksQuery>(request);

            var result = await _mediator.Send(command);

            return result.ToHttpResponse<List<GetFilteredTasksResult>>();
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var command = _mapper.Map<CreateTaskCommand>(request);

            var result = await _mediator.Send(command);

            return result.ToHttpResponse<Guid>();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
        {
            var command = new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.Assignee,
                request.Status,
                request.Priority);

            await _mediator.Send(command);
            return Unit.Value.ToHttpResponse<object>();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteTaskCommand(id));
            return Unit.Value.ToHttpResponse<object>();
        }
    }
}
