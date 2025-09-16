using MediatR;
using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetFilteredTasks;

public record GetFilteredTasksQuery : IRequest<List<GetFilteredTasksResult>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public TasksStatus? Status { get; set; }
    public TasksPriority? Priority { get; set; }
    public string? Assignee { get; set; }
}
