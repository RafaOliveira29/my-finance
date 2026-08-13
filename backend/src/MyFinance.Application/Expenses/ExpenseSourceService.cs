using FluentValidation;
using MyFinance.Application.Abstractions;
using MyFinance.Application.Common;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Expenses;

public sealed class ExpenseSourceService(
    IRepository<ExpenseSource> expenses,
    IRepository<Category> categories,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IValidator<CreateExpenseSourceRequest> createValidator,
    IValidator<UpdateExpenseSourceRequest> updateValidator) : IExpenseSourceService
{
    public async Task<ExpenseSourceResponse> CreateAsync(CreateExpenseSourceRequest request, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var source = ExpenseSource.Create(RequireUserId(), request.CategoryId, request.Description, request.ExpenseKind,
            request.DefaultAmount, request.DueDay, request.DueMonthOffset, request.RecurrenceType, request.StartDate,
            request.EndDate, request.Notes, DateTime.UtcNow);
        expenses.Add(source);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(source);
    }

    public async Task<IReadOnlyList<ExpenseSourceResponse>> ListAsync(CancellationToken cancellationToken = default) =>
        (await expenses.ListAsync(cancellationToken)).OrderBy(e => e.Description).Select(Map).ToList();

    public async Task<ExpenseSourceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Map(await expenses.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id));

    public async Task<ExpenseSourceResponse> UpdateAsync(Guid id, UpdateExpenseSourceRequest request, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var source = await expenses.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id);
        source.Update(request.CategoryId, request.Description, request.ExpenseKind, request.DefaultAmount, request.DueDay,
            request.DueMonthOffset, request.RecurrenceType, request.StartDate, request.EndDate, request.IsActive, request.Notes, DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(source);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var source = await expenses.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id);
        expenses.Remove(source);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        if (!await categories.AnyAsync(c => c.Id == categoryId, cancellationToken))
            throw new DomainValidationException("Categoria inválida ou inexistente.");
    }

    private Guid RequireUserId() => currentUser.UserId ?? throw new UnauthorizedException("Usuário não autenticado.");
    private static NotFoundException NotFound(Guid id) => new($"Fonte de despesa {id} não encontrada.");

    private static ExpenseSourceResponse Map(ExpenseSource e) => new(
        e.Id, e.CategoryId, e.Description, e.ExpenseKind, e.DefaultAmount, e.DueDay, e.DueMonthOffset,
        e.RecurrenceType, e.StartDate, e.EndDate, e.IsActive, e.Notes);
}