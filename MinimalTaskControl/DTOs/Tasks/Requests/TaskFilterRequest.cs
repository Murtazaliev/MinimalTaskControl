using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.WebApi.DTOs.Tasks.Requests;

public class TaskFilterRequest 
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public TasksStatus? Status { get; set; }
    public TasksPriority? Priority { get; set; }
    public string? Assignee { get; set; }
}
