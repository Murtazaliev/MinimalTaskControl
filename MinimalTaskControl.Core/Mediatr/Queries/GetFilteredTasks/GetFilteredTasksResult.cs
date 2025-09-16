using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetFilteredTasks;

public class GetFilteredTasksResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? Assignee { get; set; } = string.Empty;
    public TasksStatus Status { get; set; }
    public TasksPriority Priority { get; set; }
    public Guid? ParentTaskId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? ParentTaskTitle { get; set; }
    public int SubTasksCount { get; set; }
}

