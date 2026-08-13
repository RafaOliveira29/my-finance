using System.Linq.Expressions;
using MyFinance.Application.Abstractions;
using MyFinance.Domain.Abstractions;

namespace MyFinance.Application.Tests.Cadastros;

internal sealed class FakeRepository<T> : IRepository<T> where T : Entity
{
    public List<T> Items { get; } = [];

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<T>>(Items.ToList());

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult(Items.Any(predicate.Compile()));

    public void Add(T entity) => Items.Add(entity);
    public void Remove(T entity) => Items.Remove(entity);
}

internal sealed class FakeCurrentUser(Guid? userId) : ICurrentUser
{
    public Guid? UserId { get; } = userId;
    public bool IsAuthenticated => UserId is not null;
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
}