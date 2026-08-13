namespace MyFinance.Application.Expenses;

public interface IExpenseSourceService
{
    Task<ExpenseSourceResponse> CreateAsync(CreateExpenseSourceRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseSourceResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<ExpenseSourceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExpenseSourceResponse> UpdateAsync(Guid id, UpdateExpenseSourceRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}