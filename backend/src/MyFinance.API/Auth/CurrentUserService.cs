using System.Security.Claims;
using MyFinance.Application.Abstractions;

namespace MyFinance.API.Auth;

/// <summary>Lê o usuário autenticado do JWT da requisição (claim <c>sub</c> → UserId).</summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            var sub = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
