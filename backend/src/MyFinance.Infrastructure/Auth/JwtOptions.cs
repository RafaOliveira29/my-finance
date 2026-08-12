namespace MyFinance.Infrastructure.Auth;

/// <summary>Configuração do JWT (seção "Jwt"). A <see cref="SigningKey"/> vem de env em produção.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "MyFinance";
    public string Audience { get; init; } = "MyFinance";
    public string SigningKey { get; init; } = string.Empty;
    public int AccessTokenMinutes { get; init; } = 120;
}
