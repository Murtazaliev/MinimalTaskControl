using MediatR;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, GetTaskByIdResult>
{
    private readonly IRepository<TaskInfo> _repository;
    private readonly ISpecificationFactory _specFactory;

    public GetTaskByIdQueryHandler(IRepository<TaskInfo> repository,
      ISpecificationFactory specFactory)
    {
        _repository = repository;
        _specFactory = specFactory;
    }

    public async Task<GetTaskByIdResult> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = _specFactory.Create<TaskInfo>(x => x.Id == request.TaskId);
        spec.AddInclude("ParentTask");
        spec.AddInclude("SubTasks");
        spec.AddSelect(x => new GetTaskByIdResult
        {
            Id = x.Id,
            Assignee = x.Assignee,
            Description = x.Description,
            ParentTaskId = x.ParentTaskId,
            Priority = x.Priority,
            Status = x.Status,
            Title = x.Title,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,

            SubTasks = x.SubTasks.Select(st => new GetTaskByIdSubTaskResult 
            {
                Id = st.Id,
                Title = st.Title,
                Description = st.Description,
                Status = st.Status,
                Priority = st.Priority,
                CreatedAt = st.CreatedAt
            }).ToList(),

            ParentTask = x.ParentTask != null ? new GetTaskByIdParentTaskResult 
            {
                Title = x.ParentTask.Title,
                Description = x.ParentTask.Description,
                Status = x.ParentTask.Status,
                Priority = x.ParentTask.Priority,
                CreatedAt = x.ParentTask.CreatedAt
            } : null
        });

        var task = await _repository.GetFirstOrDefaultAsync<GetTaskByIdResult>(spec, cancellationToken);

        return task == null ? throw new NotFoundException("Task", request.TaskId) : task;
    }
}
