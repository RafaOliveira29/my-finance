using MyFinance.Domain.Enums;

namespace MyFinance.Application.Expenses;

public sealed record CreateExpenseSourceRequest(
    Guid CategoryId, string Description, ExpenseKind ExpenseKind, decimal DefaultAmount, int? DueDay,
    int DueMonthOffset, RecurrenceType RecurrenceType, DateOnly StartDate, DateOnly? EndDate, string? Notes);

public sealed record UpdateExpenseSourceRequest(
    Guid CategoryId, string Description, ExpenseKind ExpenseKind, decimal DefaultAmount, int? DueDay,
    int DueMonthOffset, RecurrenceType RecurrenceType, DateOnly StartDate, DateOnly? EndDate, bool IsActive, string? Notes);

public sealed record ExpenseSourceResponse(
    Guid Id, Guid CategoryId, string Description, ExpenseKind ExpenseKind, decimal DefaultAmount, int? DueDay,
    int DueMonthOffset, RecurrenceType RecurrenceType, DateOnly StartDate, DateOnly? EndDate, bool IsActive, string? Notes);