using MediatR;

namespace MinimalTaskControl.Core.Mediatr.Queries.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<GetTaskByIdResult>;
