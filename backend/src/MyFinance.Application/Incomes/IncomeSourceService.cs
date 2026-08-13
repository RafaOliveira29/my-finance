using FluentValidation;
using MyFinance.Application.Abstractions;
using MyFinance.Application.Common;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Incomes;

public sealed class IncomeSourceService(
    IRepository<IncomeSource> incomes,
    IRepository<Category> categories,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IValidator<CreateIncomeSourceRequest> createValidator,
    IValidator<UpdateIncomeSourceRequest> updateValidator) : IIncomeSourceService
{
    public async Task<IncomeSourceResponse> CreateAsync(CreateIncomeSourceRequest request, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var source = IncomeSource.Create(RequireUserId(), request.CategoryId, request.Description, request.DefaultAmount,
            request.CompetenceDay, request.RecurrenceType, request.StartDate, request.EndDate, request.Notes, DateTime.UtcNow);
        incomes.Add(source);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(source);
    }

    public async Task<IReadOnlyList<IncomeSourceResponse>> ListAsync(CancellationToken cancellationToken = default) =>
        (await incomes.ListAsync(cancellationToken)).OrderBy(i => i.Description).Select(Map).ToList();

    public async Task<IncomeSourceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Map(await incomes.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id));

    public async Task<IncomeSourceResponse> UpdateAsync(Guid id, UpdateIncomeSourceRequest request, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureCategoryExistsAsync(request.CategoryId, cancellationToken);

        var source = await incomes.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id);
        source.Update(request.CategoryId, request.Description, request.DefaultAmount, request.CompetenceDay,
            request.RecurrenceType, request.StartDate, request.EndDate, request.IsActive, request.Notes, DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(source);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var source = await incomes.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id);
        incomes.Remove(source);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        if (!await categories.AnyAsync(c => c.Id == categoryId, cancellationToken))
            throw new DomainValidationException("Categoria inválida ou inexistente.");
    }

    private Guid RequireUserId() => currentUser.UserId ?? throw new UnauthorizedException("Usuário não autenticado.");
    private static NotFoundException NotFound(Guid id) => new($"Fonte de receita {id} não encontrada.");

    private static IncomeSourceResponse Map(IncomeSource i) => new(
        i.Id, i.CategoryId, i.Description, i.DefaultAmount, i.CompetenceDay,
        i.RecurrenceType, i.StartDate, i.EndDate, i.IsActive, i.Notes);
}