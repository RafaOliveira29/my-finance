namespace MyFinance.Application.Abstractions;

/// <summary>Confirma as mudanças de uma unidade de trabalho (transação lógica).</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
