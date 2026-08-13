using FluentValidation;
using MyFinance.Application.Abstractions;
using MyFinance.Application.Common;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Categories;

public sealed class CategoryService(
    IRepository<Category> categories,
    IRepository<IncomeSource> incomes,
    IRepository<ExpenseSource> expenses,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IValidator<CreateCategoryRequest> createValidator,
    IValidator<UpdateCategoryRequest> updateValidator) : ICategoryService
{
    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var category = Category.Create(RequireUserId(), request.Name, request.Type, request.Color, request.Icon, DateTime.UtcNow);
        categories.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<IReadOnlyList<CategoryResponse>> ListAsync(CancellationToken cancellationToken = default) =>
        (await categories.ListAsync(cancellationToken)).OrderBy(c => c.Name).Select(Map).ToList();

    public async Task<CategoryResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Map(await categories.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id));

    public async Task<CategoryResponse> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var category = await categories.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id);
        category.Update(request.Name, request.Type, request.Color, request.Icon, request.IsActive, DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await categories.GetByIdAsync(id, cancellationToken) ?? throw NotFound(id);

        // Categoria em uso não pode ser apagada — só inativada (RN11 / CA065).
        var inUse = await incomes.AnyAsync(i => i.CategoryId == id, cancellationToken)
                    || await expenses.AnyAsync(e => e.CategoryId == id, cancellationToken);
        if (inUse)
            throw new ConflictException("Esta categoria está em uso e não pode ser excluída. Inative-a se não quiser mais usá-la.");

        categories.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Guid RequireUserId() => currentUser.UserId ?? throw new UnauthorizedException("Usuário não autenticado.");
    private static NotFoundException NotFound(Guid id) => new($"Categoria {id} não encontrada.");
    private static CategoryResponse Map(Category c) => new(c.Id, c.Name, c.Type, c.Color, c.Icon, c.IsActive);
}