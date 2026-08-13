namespace MyFinance.Application.Incomes;

public interface IIncomeSourceService
{
    Task<IncomeSourceResponse> CreateAsync(CreateIncomeSourceRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncomeSourceResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<IncomeSourceResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncomeSourceResponse> UpdateAsync(Guid id, UpdateIncomeSourceRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}