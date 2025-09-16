using System.ComponentModel;

namespace MinimalTaskControl.Core.Enums;

public enum TasksStatus
{
    [Description("Новый")]
    New,

    [Description("В работе")]
    InProgress,

    [Description("Выполнено")]
    Done 
}
