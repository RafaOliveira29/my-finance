using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Abstractions;
using MyFinance.Application.Auth;

namespace MyFinance.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService, ICurrentUser currentUser) : ControllerBase
{
    /// <summary>Cria uma conta (senha em hash Argon2id) e já retorna o token (CA001–CA003).</summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
        => Ok(await authService.RegisterAsync(request, cancellationToken));

    /// <summary>Autentica e retorna o token (CA003/CA004).</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));

    /// <summary>Dados do usuário autenticado — exige token válido (CA005/CA007).</summary>
    [Authorize]
    [HttpGet("me")]
    public ActionResult<UserDto> Me()
    {
        if (currentUser.UserId is not { } id)
            return Unauthorized();

        var name = User.FindFirstValue("name") ?? string.Empty;
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? string.Empty;
        return Ok(new UserDto(id, name, email));
    }
}
