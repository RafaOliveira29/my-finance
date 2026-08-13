using System.Linq.Expressions;
using MyFinance.Domain.Abstractions;

namespace MyFinance.Application.Abstractions;

/// <summary>
/// Repositório genérico para agregados. Leituras já vêm escopadas por usuário via
/// global query filter (multi-tenant) — não há como ler/alterar dado de outro usuário.
/// </summary>
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    void Add(T entity);
    void Remove(T entity);
}
