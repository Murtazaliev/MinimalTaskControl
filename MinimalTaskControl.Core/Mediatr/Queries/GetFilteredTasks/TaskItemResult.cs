using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetFilteredTasks;

public record TaskItemResult(
   Guid Id,
   string Title,
   string? Description,
   string? Assignee,
   TasksPriority Priority,
   TasksStatus Status,
   Guid? ParentTaskId,
   DateTime CreatedAt,
   DateTime? UpdatedAt,
   ICollection<TaskInfo> SubTasks,
   TaskInfo? ParentTask);
