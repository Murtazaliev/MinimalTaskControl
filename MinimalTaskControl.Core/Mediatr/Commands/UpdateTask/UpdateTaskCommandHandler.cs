using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Interfaces.Repositories;

namespace MinimalTaskControl.Core.Mediatr.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
{
    private readonly IRepository<TaskInfo> _repository;
    private readonly ITaskInfoRepository _taskInfoRepository;
    private readonly ISpecificationFactory _specFactory;

    public UpdateTaskCommandHandler(IRepository<TaskInfo> repository, ITaskInfoRepository taskInfoRepository, ISpecificationFactory specFactory)
    {
        _repository = repository;
        _taskInfoRepository = taskInfoRepository;
        _specFactory = specFactory;
    }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var spec = _specFactory.Create<TaskInfo>(x => x.Id == request.TaskId);

        var task = await _repository.GetFirstOrDefaultAsync(spec, cancellationToken) ?? throw new NotFoundException("Task", request.TaskId);

        task.SetDetails(request.Title, request.Description);
        task.SetAssignee(request.Assignee);
        task.SetPriority(request.Priority);
        task.SetStatus(request.Status);

        await _taskInfoRepository.UpdateAsync(task);
        await _taskInfoRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
