using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Interfaces.Repositories;

namespace MinimalTaskControl.Core.Mediatr.Commands.UpdateTask;

public class UpdateTaskCommandHandler(IRepository<TaskInfo> repository, ITaskInfoRepository taskInfoRepository, ISpecificationFactory specFactory) : IRequestHandler<UpdateTaskCommand>
{

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var spec = specFactory.Create<TaskInfo>(x => x.Id == request.TaskId);

        var task = await repository.GetFirstOrDefaultAsync(spec, cancellationToken) ?? throw new NotFoundException("Task", request.TaskId);

        task.SetDetails(request.Title, request.Description);
        task.SetAssignee(request.Assignee);
        task.SetPriority(request.Priority);
        task.SetStatus(request.Status);

        await taskInfoRepository.UpdateAsync(task, cancellationToken);
        await taskInfoRepository.SaveChangesAsync(cancellationToken);
    }
}
