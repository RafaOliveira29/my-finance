using MyFinance.Domain.Abstractions;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

/// <summary>Classificação de receita ou despesa (com cor/ícone). Não é tipo de despesa nem forma de pagamento.</summary>
public sealed class Category : TenantEntity
{
    public string Name { get; private set; } = null!;
    public CategoryType Type { get; private set; }
    public string? Color { get; private set; }
    public string? Icon { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Category() { } // EF Core

    private Category(Guid id, Guid userId, string name, CategoryType type, string? color, string? icon, DateTime nowUtc)
        : base(id, userId)
    {
        Name = name;
        Type = type;
        Color = color;
        Icon = icon;
        IsActive = true;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    public static Category Create(Guid userId, string name, CategoryType type, string? color, string? icon, DateTime nowUtc)
    {
        name = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório.", nameof(name));

        return new Category(Guid.NewGuid(), userId, name, type, Clean(color), Clean(icon), nowUtc);
    }

    public void Update(string name, CategoryType type, string? color, string? icon, bool isActive, DateTime nowUtc)
    {
        name = (name ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome é obrigatório.", nameof(name));

        Name = name;
        Type = type;
        Color = Clean(color);
        Icon = Clean(icon);
        IsActive = isActive;
        UpdatedAt = nowUtc;
    }

    public void Deactivate(DateTime nowUtc)
    {
        IsActive = false;
        UpdatedAt = nowUtc;
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}