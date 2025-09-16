using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetTaskById;

public class GetTaskByIdSubTaskResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? Assignee { get; set; }
    public TasksPriority Priority { get; set; }
    public TasksStatus Status { get; set; }
    public Guid? ParentTaskId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
