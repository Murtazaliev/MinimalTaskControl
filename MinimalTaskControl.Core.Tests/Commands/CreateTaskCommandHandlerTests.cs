using MinimalTaskControl.Core.Entities;
using MinimalTaskControl.Core.Enums;
using MinimalTaskControl.Core.Interfaces.Repositories;
using MinimalTaskControl.Core.Mediatr.Commands.CreateTask;
using Moq;

namespace MinimalTaskControl.Core.Tests.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskInfoRepository> _repositoryMock;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITaskInfoRepository>();
        _handler = new CreateTaskCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTaskAndReturnsId()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Test Title",
            "Test Description",
            "Test Author",
            "Test Assignee",
            TasksPriority.Medium,
            null
        );

        var createdTaskId = Guid.NewGuid();
        TaskInfo capturedTask = new();

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Callback((TaskInfo task, CancellationToken ct) => capturedTask = task)
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(capturedTask);
        Assert.Equal(command.Title, capturedTask.Title);
        Assert.Equal(command.Description, capturedTask.Description);
        Assert.Equal(command.Author, capturedTask.Author);
        Assert.Equal(command.Assignee, capturedTask.Assignee);
        Assert.Equal(command.Priority, capturedTask.Priority);
        Assert.Equal(command.ParentTaskId, capturedTask.ParentTaskId);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_WithParentTaskId_CreatesTaskWithParent()
    {
        // Arrange
        var parentTaskId = Guid.NewGuid();
        var command = new CreateTaskCommand(
            "Test Title",
            "Test Description",
            "Test Author",
            "Test Assignee",
            TasksPriority.High,
            parentTaskId
        );

        TaskInfo capturedTask = new();

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Callback((TaskInfo task, CancellationToken ct) => capturedTask = task)
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedTask);
        Assert.Equal(parentTaskId, capturedTask.ParentTaskId);
    }

    [Fact]
    public async Task Handle_WithDifferentPriorities_CreatesTaskWithCorrectPriority()
    {
        // Arrange
        var priorities = new[] { TasksPriority.Low, TasksPriority.Medium, TasksPriority.High };

        foreach (var priority in priorities)
        {
            var command = new CreateTaskCommand(
                "Test Title",
                "Test Description",
                "Test Author",
                "Test Assignee",
                priority,
                null
            );

            TaskInfo capturedTask = new();

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
                .Callback((TaskInfo task, CancellationToken ct) => capturedTask = task)
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedTask);
            Assert.Equal(priority, capturedTask.Priority);

            _repositoryMock.Invocations.Clear();
        }
    }

    [Fact]
    public async Task Handle_CallsRepositoryMethodsInCorrectOrder()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Test Title",
            "Test Description",
            "Test Author",
            "Test Assignee",
            TasksPriority.Medium,
            null
        );

        var callOrder = new List<string>();

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("AddAsync"))
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("SaveChangesAsync"))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("AddAsync", callOrder[0]);
        Assert.Equal("SaveChangesAsync", callOrder[1]);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ExceptionIsPropagated()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Test Title",
            "Test Description",
            "Test Author",
            "Test Assignee",
            TasksPriority.Medium,
            null
        );

        var expectedException = new InvalidOperationException("Database error");

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task Handle_WithEmptyStrings_ThrowsArgumentExceptionForTitle()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "", 
            "", 
            "", 
            "", 
            TasksPriority.Low,
            null
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal("title", exception.ParamName);
        Assert.Contains("Название задачи не может быть пустым", exception.Message);
    }

    [Fact]
    public async Task Handle_VerifyCancellationTokenIsPassed()
    {
        // Arrange
        var command = new CreateTaskCommand(
            "Test Title",
            "Test Description",
            "Test Author",
            "Test Assignee",
            TasksPriority.Medium,
            null
        );

        var cancellationToken = new CancellationToken(true); // Cancelled token

        TaskInfo capturedTask = new();
        CancellationToken capturedToken = CancellationToken.None;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskInfo>(), It.IsAny<CancellationToken>()))
            .Callback((TaskInfo task, CancellationToken ct) =>
            {
                capturedTask = task;
                capturedToken = ct;
            })
            .Returns(Task.CompletedTask);

        _repositoryMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(capturedToken.IsCancellationRequested);
    }
}
