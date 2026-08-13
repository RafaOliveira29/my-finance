using MyFinance.Domain.Enums;

namespace MyFinance.Application.Incomes;

public sealed record CreateIncomeSourceRequest(
    Guid CategoryId, string Description, decimal DefaultAmount, int CompetenceDay,
    RecurrenceType RecurrenceType, DateOnly StartDate, DateOnly? EndDate, string? Notes);

public sealed record UpdateIncomeSourceRequest(
    Guid CategoryId, string Description, decimal DefaultAmount, int CompetenceDay,
    RecurrenceType RecurrenceType, DateOnly StartDate, DateOnly? EndDate, bool IsActive, string? Notes);

public sealed record IncomeSourceResponse(
    Guid Id, Guid CategoryId, string Description, decimal DefaultAmount, int CompetenceDay,
    RecurrenceType RecurrenceType, DateOnly StartDate, DateOnly? EndDate, bool IsActive, string? Notes);