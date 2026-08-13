using MyFinance.Domain.Abstractions;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

/// <summary>Origem estrutural de receita (o "molde" recorrente: salário, renda extra). Não é o recebimento.</summary>
public sealed class IncomeSource : TenantEntity
{
    public Guid CategoryId { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal DefaultAmount { get; private set; }
    /// <summary>Dia do mês esperado do recebimento (1..31).</summary>
    public int CompetenceDay { get; private set; }
    public RecurrenceType RecurrenceType { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private IncomeSource() { } // EF Core

    private IncomeSource(
        Guid id, Guid userId, Guid categoryId, string description, decimal defaultAmount,
        int competenceDay, RecurrenceType recurrenceType, DateOnly startDate, DateOnly? endDate,
        string? notes, DateTime nowUtc) : base(id, userId)
    {
        CategoryId = categoryId;
        Description = description;
        DefaultAmount = defaultAmount;
        CompetenceDay = competenceDay;
        RecurrenceType = recurrenceType;
        StartDate = startDate;
        EndDate = endDate;
        Notes = notes;
        IsActive = true;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    public static IncomeSource Create(
        Guid userId, Guid categoryId, string description, decimal defaultAmount, int competenceDay,
        RecurrenceType recurrenceType, DateOnly startDate, DateOnly? endDate, string? notes, DateTime nowUtc)
    {
        Validate(categoryId, ref description, defaultAmount, competenceDay, startDate, endDate);
        return new IncomeSource(Guid.NewGuid(), userId, categoryId, description, defaultAmount,
            competenceDay, recurrenceType, startDate, endDate, Clean(notes), nowUtc);
    }

    public void Update(
        Guid categoryId, string description, decimal defaultAmount, int competenceDay,
        RecurrenceType recurrenceType, DateOnly startDate, DateOnly? endDate, bool isActive, string? notes, DateTime nowUtc)
    {
        Validate(categoryId, ref description, defaultAmount, competenceDay, startDate, endDate);
        CategoryId = categoryId;
        Description = description;
        DefaultAmount = defaultAmount;
        CompetenceDay = competenceDay;
        RecurrenceType = recurrenceType;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
        Notes = Clean(notes);
        UpdatedAt = nowUtc;
    }

    public void Deactivate(DateTime nowUtc)
    {
        IsActive = false;
        UpdatedAt = nowUtc;
    }

    private static void Validate(Guid categoryId, ref string description, decimal defaultAmount, int competenceDay, DateOnly startDate, DateOnly? endDate)
    {
        description = (description ?? string.Empty).Trim();
        if (categoryId == Guid.Empty) throw new ArgumentException("Categoria é obrigatória.", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Descrição é obrigatória.", nameof(description));
        if (defaultAmount < 0) throw new ArgumentException("Valor não pode ser negativo.", nameof(defaultAmount));
        if (competenceDay is < 1 or > 31) throw new ArgumentException("Dia de competência deve estar entre 1 e 31.", nameof(competenceDay));
        if (endDate is not null && endDate < startDate) throw new ArgumentException("Data fim não pode ser anterior ao início.", nameof(endDate));
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
