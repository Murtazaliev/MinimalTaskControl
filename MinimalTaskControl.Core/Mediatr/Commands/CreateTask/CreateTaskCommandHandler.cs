using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Interfaces.Repositories;

namespace MinimalTaskControl.Core.Mediatr.Commands.CreateTask
{
    public class CreateTaskCommandHandler(ITaskInfoRepository taskInfoRepository) : IRequestHandler<CreateTaskCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = new TaskInfo(
                request.Title,
                request.Description,
                request.Author,
                request.Assignee,
                request.Priority,
                request.ParentTaskId
            );

            await taskInfoRepository.AddAsync(task, cancellationToken);

            await taskInfoRepository.SaveChangesAsync(cancellationToken);

            return task.Id;
        }
    }
}
