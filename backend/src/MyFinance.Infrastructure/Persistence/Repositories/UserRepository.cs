using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Abstractions;
using MyFinance.Domain.Entities;

namespace MyFinance.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(MyFinanceDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public void Add(User user) => db.Users.Add(user);
}
