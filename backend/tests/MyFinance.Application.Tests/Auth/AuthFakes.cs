using MyFinance.Application.Abstractions;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Tests.Auth;

internal sealed class FakeUserRepository : IUserRepository
{
    public List<User> Saved { get; } = [];

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Saved.FirstOrDefault(u => u.Email == email));

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Saved.Any(u => u.Email == email));

    public void Add(User user) => Saved.Add(user);
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => "hashed:" + password;
    public bool Verify(string hash, string password) => hash == "hashed:" + password;
}

internal sealed class FakeTokenService : ITokenService
{
    public TokenResult Generate(User user) => new("token-for-" + user.Id, DateTime.UtcNow.AddHours(1));
}
