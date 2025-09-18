using MediatR;
using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Mediatr.Commands.UpdateTask;

public record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description,
    string? Assignee,
    TasksStatus? Status,
    TasksPriority? Priority) : IRequest;
