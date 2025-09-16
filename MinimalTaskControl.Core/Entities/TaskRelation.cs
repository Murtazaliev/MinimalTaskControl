using MinimalTaskControl.Core.Interfaces;

namespace MinimalTaskControl.Core.Entities;

public class TaskRelation : IEntityMarker
{
    /// <summary>
 /// Уникальный идентификатор связи.
 /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Идентификатор основной задачи в связи.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Основная задача, из которой исходит связь.
    /// </summary>
    public TaskInfo Task { get; set; } = null!; 

    /// <summary>
    /// Идентификатор связанной задачи.
    /// </summary>
    public Guid RelatedTaskId { get; set; }

    /// <summary>
    /// Задача, которая связана с основной.
    /// </summary>
    public TaskInfo RelatedTask { get; set; } = null!;

    /// <summary>
    /// Дата создания связи
    /// </summary>
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
