using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Interfaces;
using System.Linq.Expressions;
using MinimalTaskControl.Core.Exceptions;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetFilteredTasks;

public class GetFilteredTasksQueryHandler : IRequestHandler<GetFilteredTasksQuery, List<GetFilteredTasksResult>>
{
    private readonly IRepository<TaskInfo> _repository;
    private readonly ISpecificationFactory _specFactory;

    public GetFilteredTasksQueryHandler(
        IRepository<TaskInfo> repository,
        ISpecificationFactory specFactory)
    {
        _repository = repository;
        _specFactory = specFactory;
    }

    public async Task<List<GetFilteredTasksResult>> Handle(
        GetFilteredTasksQuery request,
        CancellationToken cancellationToken)
    {
        // Создаем базовую спецификацию
        var criteria = BuildCriteria(request);

        var spec = _specFactory.Create(criteria);
        spec.AddOrderByDescending(x => x.CreatedAt);
        spec.AddInclude("ParentTask");
        spec.AddInclude("SubTasks");

        spec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

        spec.AddSelect(x => new GetFilteredTasksResult
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Assignee = x.Assignee,
            Status = x.Status,
            Priority = x.Priority,
            ParentTaskId = x.ParentTaskId,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            ParentTaskTitle = x.ParentTask != null ? x.ParentTask.Title : null,
            SubTasksCount = x.SubTasks != null ? x.SubTasks.Count : 0
        });

        return await _repository.ListAsync<GetFilteredTasksResult>(spec, cancellationToken) ?? throw new NotFoundException(nameof(GetFilteredTasksResult));
    }

    private static Expression<Func<TaskInfo, bool>> BuildCriteria(GetFilteredTasksQuery request)
    {
        return x =>
            (string.IsNullOrEmpty(request.SearchTerm) ||
             x.Title.Contains(request.SearchTerm) || (!string.IsNullOrEmpty(x.Description) && x.Description.Contains(request.SearchTerm))) &&
            (!request.Status.HasValue || x.Status == request.Status.Value) &&
            (!request.Priority.HasValue || x.Priority == request.Priority.Value) &&
            (string.IsNullOrEmpty(request.Assignee) || x.Assignee == request.Assignee);
    }
}
