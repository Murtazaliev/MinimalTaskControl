namespace MinimalTaskControl.Core.Enums;

public enum CoreErrorType
{
    /// <summary>
    /// Ошибка отсутствует.
    /// </summary>
    None,

    /// <summary>
    /// Базовое исключение.
    /// </summary>
    Exception,

    /// <summary>
    /// Общая ошибка.
    /// </summary>
    Error,

    /// <summary>
    /// Не найден объект.
    /// </summary>
    NotFound,

    /// <summary>
    /// Ошибка валидации.
    /// </summary>
    Validation,

    /// <summary>
    /// Отсутствует доступ.
    /// </summary>
    AccessDenied
}
