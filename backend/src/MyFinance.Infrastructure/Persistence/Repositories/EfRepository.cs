using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Abstractions;
using MyFinance.Domain.Abstractions;

namespace MyFinance.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repositório genérico EF Core. Para <see cref="TenantEntity"/>, TODA consulta é escopada pelo
/// usuário atual (RN13) — o UserId é lido a cada query (não fica preso no modelo cacheado do EF),
/// então não há vazamento entre usuários nem IDOR (buscar por Id fora do escopo devolve null).
/// </summary>
public sealed class EfRepository<T>(MyFinanceDbContext db, ICurrentUser currentUser) : IRepository<T> where T : Entity
{
    private static readonly bool IsTenantScoped = typeof(TenantEntity).IsAssignableFrom(typeof(T));

    private IQueryable<T> Scoped()
    {
        IQueryable<T> query = db.Set<T>();
        if (IsTenantScoped)
        {
            var userId = currentUser.UserId ?? Guid.Empty;
            query = query.Where(e => EF.Property<Guid>(e, "UserId") == userId);
        }
        return query;
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Scoped().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) =>
        await Scoped().AsNoTracking().ToListAsync(cancellationToken);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        Scoped().AnyAsync(predicate, cancellationToken);

    public void Add(T entity) => db.Set<T>().Add(entity);

    public void Remove(T entity) => db.Set<T>().Remove(entity);
}