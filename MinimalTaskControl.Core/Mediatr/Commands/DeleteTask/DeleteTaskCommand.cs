using MediatR;

namespace MinimalTaskControl.Core.Mediatr.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId) : IRequest<Unit>;
