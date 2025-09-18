using MediatR;
using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Enums;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.Core.Mediatr.Commands.DeleteTask;
using Moq;
using System.Linq.Expressions;

namespace MinimalTaskControl.Core.Tests.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<IRepository<TaskInfo>> _repositoryMock;
    private readonly Mock<ITaskInfoRepository> _taskInfoRepositoryMock;
    private readonly Mock<ISpecificationFactory> _specFactoryMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<IRepository<TaskInfo>>();
        _taskInfoRepositoryMock = new Mock<ITaskInfoRepository>();
        _specFactoryMock = new Mock<ISpecificationFactory>();
        _handler = new DeleteTaskCommandHandler(
            _repositoryMock.Object,
            _taskInfoRepositoryMock.Object,
            _specFactoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidTaskId_MarksTaskAsDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);

        var task = new TaskInfo("Test", "Desc", "Author", "Assignee", TasksPriority.Medium, null);
        var spec = new Mock<ISpecification<TaskInfo>>();

        _specFactoryMock
            .Setup(f => f.Create<TaskInfo>(It.IsAny<Expression<Func<TaskInfo, bool>>>()))
            .Returns(spec.Object);

        _repositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(spec.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _taskInfoRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskInfoRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _taskInfoRepositoryMock.Verify(r => r.UpdateAsync(It.Is<TaskInfo>(t => t.DeletedAt != null), It.IsAny<CancellationToken>()), Times.Once);
        _taskInfoRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);
        var spec = new Mock<ISpecification<TaskInfo>>();

        _specFactoryMock
            .Setup(f => f.Create(It.IsAny<Expression<Func<TaskInfo, bool>>>()))
            .Returns(spec.Object);

        _repositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(spec.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskInfo?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_TaskWithActiveSubtasks_ThrowsBusinessException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);

        var task = new TaskInfo("Test", "Desc", "Author", "Assignee", TasksPriority.Medium, null);
        var subTask = new TaskInfo("Sub", "Desc", "Author", "Assignee", TasksPriority.Low, taskId);
        task.SetSubTasks([subTask]);

        var spec = new Mock<ISpecification<TaskInfo>>();

        _specFactoryMock
            .Setup(f => f.Create<TaskInfo>(It.IsAny<Expression<Func<TaskInfo, bool>>>()))
            .Returns(spec.Object);

        _repositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(spec.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("Невозможно удалить задачу с активными подзадачами", exception.Message);
    }

    [Fact]
    public async Task Handle_TaskWithDeletedSubtasks_DeletesSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);

        var task = new TaskInfo("Test", "Desc", "Author", "Assignee", TasksPriority.Medium, null);
        var subTask = new TaskInfo("Sub", "Desc", "Author", "Assignee", TasksPriority.Low, taskId);
        subTask.MarkAsDeleted(); // Удаленная подзадача
        task.SetSubTasks([subTask]);

        var spec = new Mock<ISpecification<TaskInfo>>();

        _specFactoryMock
            .Setup(f => f.Create<TaskInfo>(It.IsAny<Expression<Func<TaskInfo, bool>>>()))
            .Returns(spec.Object);

        _repositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(spec.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _taskInfoRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskInfoRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _taskInfoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_AlreadyDeletedTask_DeletesSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);

        var task = new TaskInfo("Test", "Desc", "Author", "Assignee", TasksPriority.Medium, null);
        task.MarkAsDeleted(); 

        var spec = new Mock<ISpecification<TaskInfo>>();

        _specFactoryMock
            .Setup(f => f.Create(It.IsAny<Expression<Func<TaskInfo, bool>>>()))
            .Returns(spec.Object);

        _repositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(spec.Object, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        _taskInfoRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskInfoRepositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _taskInfoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(Unit.Value, result);
    }
}
