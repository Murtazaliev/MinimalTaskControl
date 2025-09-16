using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Interfaces.Repositories;

namespace MinimalTaskControl.Core.Mediatr.Commands.CreateTask
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly ITaskInfoRepository _taskInfoRepository;
        public CreateTaskCommandHandler(ITaskInfoRepository taskInfoRepository)
        {
            _taskInfoRepository = taskInfoRepository;
        }

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

            await _taskInfoRepository.AddAsync(task, cancellationToken);

            await _taskInfoRepository.SaveChangesAsync(cancellationToken);

            return task.Id;
        }
    }
}
