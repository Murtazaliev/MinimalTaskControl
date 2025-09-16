using System.ComponentModel;

namespace MinimalTaskControl.Core.Enums;

public enum TasksPriority 
{
    [Description("Низкий")]
    Low,

    [Description("Средний")]
    Medium,

    [Description("Высокий")]
    High 
}
