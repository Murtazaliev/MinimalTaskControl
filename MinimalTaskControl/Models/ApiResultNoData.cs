using MinimalTaskControl.WebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MinimalTaskControl.WebApi.Models;

public class ApiResultNoData
{
    /// <summary>
    /// Тип ошибки
    /// </summary>
    [Required]
    public ErrorType Error { get; set; }

    /// <summary>
    /// Параметры запроса.
    /// </summary>
    public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();

    /// <summary>
    ///     Комментарий.
    /// </summary>
    public string? Comment { get; set; }
}
