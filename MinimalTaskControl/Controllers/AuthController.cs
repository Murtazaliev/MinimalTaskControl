using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalTaskControl.Core.Interfaces;
using MinimalTaskControl.WebApi.Models;
using MinimalTaskControl.WebApi.Models.Auth;

namespace MinimalTaskControl.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase 
{
    private readonly IJwtService _jwtService;
    private readonly List<AuthUser> _authUsers = new();

    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;

        _authUsers.Add(new AuthUser { Username = "admin", Password = "admin", Role = "Admin" });
        _authUsers.Add(new AuthUser { Username = "user", Password = "user", Role = "User" });
        _authUsers.Add(new AuthUser { Username = "string", Password = "string", Role = "User" });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _authUsers.FirstOrDefault(u =>
            u.Username == request.Username && u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        var token = _jwtService.GenerateToken(user.Username, user.Role);
        return Ok(new AuthResponse { Token = token, Username = user.Username, Role = user.Role });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (_authUsers.Any(u => u.Username == request.Username))
        {
            return BadRequest("User already exists");
        }

        var newUser = new AuthUser // ← Создаем AuthUser
        {
            Username = request.Username,
            Password = request.Password,
            Role = "User"
        };

        _authUsers.Add(newUser);

        var token = _jwtService.GenerateToken(newUser.Username, newUser.Role);
        return Ok(new AuthResponse { Token = token, Username = newUser.Username, Role = newUser.Role });
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUsers()
    {
        return Ok(_authUsers.Select(u => new { u.Username, u.Role }));
    }
}