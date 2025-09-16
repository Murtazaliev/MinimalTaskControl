using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.WebApi.DTOs.Tasks.Requests;

public class UpdateTaskRequest : BaseTaskInfo
{
    public TasksStatus? Status { get; set; }
}
