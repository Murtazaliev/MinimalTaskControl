using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Mediatr.Commands.UpdateTask;

namespace MinimalTaskControl.Core.Mediatr.Commands.DeleteTask;

public class DeleteTaskCommandHandler(IRepository<TaskInfo> repository, ITaskInfoRepository taskInfoRepository, ISpecificationFactory specFactory)
{
    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var spec = specFactory.Create<TaskInfo>(x => x.Id == request.TaskId);
        spec.AddInclude("SubTasks");

        var task = await repository.GetFirstOrDefaultAsync(spec, cancellationToken) ?? throw new NotFoundException("Task", request.TaskId);

        if (task.SubTasks != null && task.SubTasks.Any(st => st.DeletedAt == null))
        {
            throw new BusinessException("Невозможно удалить задачу с активными подзадачами");
        }

        task.MarkAsDeleted();

        await taskInfoRepository.UpdateAsync(task, cancellationToken);
        await taskInfoRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
