using MediatR;
using MinimalTaskControl.Core.Enums;

namespace MinimalTaskControl.Core.Mediatr.Commands.CreateTask;

public record CreateTaskCommand(string Title, string Description, string Author, string Assignee, TasksPriority Priority, Guid? ParentTaskId) : IRequest<Guid>;

