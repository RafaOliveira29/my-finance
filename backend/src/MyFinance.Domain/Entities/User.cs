using MyFinance.Domain.Abstractions;

namespace MyFinance.Domain.Entities;

/// <summary>
/// Raiz multi-tenant: dono de todos os dados financeiros. Rica (setters privados + factory),
/// nunca criada em estado inválido. A senha entra já como <b>hash</b> — o domínio não conhece
/// texto puro nem o algoritmo de hashing (isso é infraestrutura).
/// </summary>
public sealed class User : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User() { } // EF Core

    private User(Guid id, string name, string email, string passwordHash, DateTime nowUtc) : base(id)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    public static User Create(string name, string email, string passwordHash, DateTime nowUtc)
    {
        name = (name ?? string.Empty).Trim();
        email = NormalizeEmail(email);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail é obrigatório.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Hash de senha é obrigatório.", nameof(passwordHash));

        return new User(Guid.NewGuid(), name, email, passwordHash, nowUtc);
    }

    public void ChangePassword(string newPasswordHash, DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Hash de senha é obrigatório.", nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
        UpdatedAt = nowUtc;
    }

    /// <summary>E-mail é sempre normalizado (trim + minúsculas) — base da unicidade.</summary>
    public static string NormalizeEmail(string email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
