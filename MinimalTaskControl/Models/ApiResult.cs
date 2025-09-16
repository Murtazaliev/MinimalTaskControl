using MinimalTaskControl.WebApi.Enums;

namespace MinimalTaskControl.WebApi.Models;

public class ApiResult<TData>
{
    public TData? Data { get; set; }

    public ErrorType Error { get; set; }

    public Dictionary<string, string>? Params { get; set; }

    public string? Comment { get; set; }
}
