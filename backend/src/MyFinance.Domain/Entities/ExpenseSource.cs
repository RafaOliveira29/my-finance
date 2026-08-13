using MyFinance.Domain.Abstractions;
using MyFinance.Domain.Enums;

namespace MyFinance.Domain.Entities;

/// <summary>Origem estrutural de despesa recorrente (aluguel, internet…). Parcelamento vive na Dívida, não aqui.</summary>
public sealed class ExpenseSource : TenantEntity
{
    public Guid CategoryId { get; private set; }
    public string Description { get; private set; } = null!;
    public ExpenseKind ExpenseKind { get; private set; }
    public decimal DefaultAmount { get; private set; }
    /// <summary>Dia do vencimento no mês (1..31), opcional.</summary>
    public int? DueDay { get; private set; }
    /// <summary>0 = vence no mês da competência; 1 = vence no mês seguinte (competência ≠ vencimento, RN01).</summary>
    public int DueMonthOffset { get; private set; }
    public RecurrenceType RecurrenceType { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ExpenseSource() { } // EF Core

    private ExpenseSource(
        Guid id, Guid userId, Guid categoryId, string description, ExpenseKind expenseKind, decimal defaultAmount,
        int? dueDay, int dueMonthOffset, RecurrenceType recurrenceType, DateOnly startDate, DateOnly? endDate,
        string? notes, DateTime nowUtc) : base(id, userId)
    {
        CategoryId = categoryId;
        Description = description;
        ExpenseKind = expenseKind;
        DefaultAmount = defaultAmount;
        DueDay = dueDay;
        DueMonthOffset = dueMonthOffset;
        RecurrenceType = recurrenceType;
        StartDate = startDate;
        EndDate = endDate;
        Notes = notes;
        IsActive = true;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    public static ExpenseSource Create(
        Guid userId, Guid categoryId, string description, ExpenseKind expenseKind, decimal defaultAmount,
        int? dueDay, int dueMonthOffset, RecurrenceType recurrenceType, DateOnly startDate, DateOnly? endDate,
        string? notes, DateTime nowUtc)
    {
        Validate(categoryId, ref description, defaultAmount, dueDay, dueMonthOffset, startDate, endDate);
        return new ExpenseSource(Guid.NewGuid(), userId, categoryId, description, expenseKind, defaultAmount,
            dueDay, dueMonthOffset, recurrenceType, startDate, endDate, Clean(notes), nowUtc);
    }

    public void Update(
        Guid categoryId, string description, ExpenseKind expenseKind, decimal defaultAmount, int? dueDay,
        int dueMonthOffset, RecurrenceType recurrenceType, DateOnly startDate, DateOnly? endDate, bool isActive,
        string? notes, DateTime nowUtc)
    {
        Validate(categoryId, ref description, defaultAmount, dueDay, dueMonthOffset, startDate, endDate);
        CategoryId = categoryId;
        Description = description;
        ExpenseKind = expenseKind;
        DefaultAmount = defaultAmount;
        DueDay = dueDay;
        DueMonthOffset = dueMonthOffset;
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

    private static void Validate(Guid categoryId, ref string description, decimal defaultAmount, int? dueDay, int dueMonthOffset, DateOnly startDate, DateOnly? endDate)
    {
        description = (description ?? string.Empty).Trim();
        if (categoryId == Guid.Empty) throw new ArgumentException("Categoria é obrigatória.", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Descrição é obrigatória.", nameof(description));
        if (defaultAmount < 0) throw new ArgumentException("Valor não pode ser negativo.", nameof(defaultAmount));
        if (dueDay is not null && dueDay is < 1 or > 31) throw new ArgumentException("Dia de vencimento deve estar entre 1 e 31.", nameof(dueDay));
        if (dueMonthOffset is < 0 or > 1) throw new ArgumentException("DueMonthOffset deve ser 0 ou 1.", nameof(dueMonthOffset));
        if (endDate is not null && endDate < startDate) throw new ArgumentException("Data fim não pode ser anterior ao início.", nameof(endDate));
    }

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
