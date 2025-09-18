using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Interfaces.Repositories;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetTaskById;

public class GetTaskByIdQueryHandler(IRepository<TaskInfo> repository,
  ISpecificationFactory specFactory) : IRequestHandler<GetTaskByIdQuery, GetTaskByIdResult>
{
    public async Task<GetTaskByIdResult> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = specFactory.Create<TaskInfo>(x => x.Id == request.TaskId);
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

        var task = await repository.GetFirstOrDefaultAsync<GetTaskByIdResult>(spec, cancellationToken);

        return task ?? throw new NotFoundException("Task", request.TaskId);
    }
}
