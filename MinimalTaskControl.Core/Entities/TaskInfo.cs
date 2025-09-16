using MinimalTaskControl.Core.Enums;
using MinimalTaskControl.Core.Exceptions;
using MinimalTaskControl.Core.Extensions;
using MinimalTaskControl.Core.Interfaces;

namespace MinimalTaskControl.Core.Entities;

public class TaskInfo : IEntityMarker
{
    /// <summary>
    /// Уникальный идентификатор задачи.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Название задачи.
    /// </summary>
    public string Title { get; private set; } = default!;

    /// <summary>
    /// Описание задачи.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Автор задачи (создатель).
    /// </summary>
    public string Author { get; private set; } = default!;

    /// <summary>
    /// Назначенный исполнитель задачи.
    /// </summary>
    public string? Assignee { get; private set; }

    /// <summary>
    /// Текущий статус задачи
    /// </summary>
    public TasksStatus Status { get; private set; } = TasksStatus.New;

    /// <summary>
    /// Приоритет задачи 
    /// </summary>
    public TasksPriority Priority { get; private set; } = TasksPriority.Medium;

    /// <summary>
    /// Внешний ключ к родительской задаче (если задача является подзадачей).
    /// </summary>
    public Guid? ParentTaskId { get; private set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    /// <summary>
    /// Родительская задача, если текущая задача вложена.
    /// </summary>
    public TaskInfo? ParentTask { get; private set; }

    /// <summary>
    /// Коллекция подзадач для текущей задачи.
    /// </summary>
    public ICollection<TaskInfo> SubTasks { get; private set; } = new List<TaskInfo>();

    /// <summary>
    /// Коллекция связанных задач через связи типа "связана с" (RelatedTasks).
    /// </summary>
    public ICollection<TaskRelation> RelatedTasks { get; private set; } = new List<TaskRelation>();

    public TaskInfo() { }
    public TaskInfo(string title, string? description, string author, string? assignee, TasksPriority priority, Guid? parentTaskId)
    {
        Id = Guid.NewGuid();
        SetDetails(title, description);
        Author = author ?? throw new ArgumentNullException(nameof(author));
        Assignee = assignee;
        Priority = priority;
        ParentTaskId = parentTaskId;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsUpdate()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        DeletedAt = DateTime.UtcNow;
    }

        public void SetDetails(string title, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название задачи не может быть пустым", nameof(title));

        Title = title;
        Description = description;
    }

    public void SetAssignee(string? assignee)
    {
        Assignee = assignee;
    }

    public void SetPriority(TasksPriority? priority)
    {
        if (priority == null)
            return;
        Priority = (TasksPriority)priority;
    }

    public void SetParentTaskId(Guid parentTaskId)
    {
        if (parentTaskId == Id)
            throw new BusinessException("Задача не может быть родительской для самой себя");

        ParentTaskId = parentTaskId;
    }

    public void SetStatus(TasksStatus? newStatus)
    {
        if (newStatus == null || Status == newStatus)
            return; 

        else if (newStatus == TasksStatus.Done)
        {
            if (SubTasks.Any(t => t.Status != TasksStatus.Done))
                throw new BusinessException("Не все подзадачи выполнены");
        }
       
        ValidateAndSetStatus((TasksStatus)newStatus);
    }

    private void ValidateAndSetStatus(TasksStatus status)
    {
        if (!IsStatusTransitionValid(status))
        {
            throw new BusinessException(
                $"Не разрешено переводить задачу из статуса " +
                $"\"{Status.ToEnumDescription()}\" в \"{status.ToEnumDescription()}\"");
        }

        Status = status;
        MarkAsUpdate();
    }

    private bool IsStatusTransitionValid(TasksStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (TasksStatus.New, TasksStatus.InProgress) => true,
            (TasksStatus.InProgress, TasksStatus.Done) => true,
            _ => false
        };
    }
}
