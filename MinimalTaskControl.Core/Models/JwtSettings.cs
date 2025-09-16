namespace MinimalTaskControl.Core.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = "MinimalTaskControlSuperSecretKey2024!";
    public string Issuer { get; set; } = "MinimalTaskControl";
    public string Audience { get; set; } = "MinimalTaskControlUsers";
    public int ExpiryMinutes { get; set; } = 60;
}