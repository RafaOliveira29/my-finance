using MyFinance.Domain.Entities;

namespace MyFinance.Application.Abstractions;

public sealed record TokenResult(string AccessToken, DateTime ExpiresAtUtc);

/// <summary>Emite o token de acesso (JWT) do usuário autenticado.</summary>
public interface ITokenService
{
    TokenResult Generate(User user);
}
