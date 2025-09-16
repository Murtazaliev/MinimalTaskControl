using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Mediatr.Commands.UpdateTask;

namespace MinimalTaskControl.Core.Mediatr.Commands.DeleteTask;

public class DeleteTaskCommandHandler
{
    private readonly IRepository<TaskInfo> _repository;
    private readonly ITaskInfoRepository _taskInfoRepository;
    private readonly ISpecificationFactory _specFactory;

    public DeleteTaskCommandHandler(IRepository<TaskInfo> repository, ITaskInfoRepository taskInfoRepository, ISpecificationFactory specFactory)
    {
        _repository = repository;
        _taskInfoRepository = taskInfoRepository;
        _specFactory = specFactory;
    }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var spec = _specFactory.Create<TaskInfo>(x => x.Id == request.TaskId);

        var task = await _repository.GetFirstOrDefaultAsync(spec, cancellationToken) ?? throw new NotFoundException("Task", request.TaskId);

        task.MarkAsDeleted();

        await _taskInfoRepository.UpdateAsync(task);
        await _taskInfoRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
