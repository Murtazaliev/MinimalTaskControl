namespace MinimalTaskControl.WebApi.Models.Auth;

/// <summary>
/// Модель для in-memory аутентификации
/// </summary>
public class AuthUser
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
