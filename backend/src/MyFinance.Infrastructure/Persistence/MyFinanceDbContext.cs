using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Abstractions;
using MyFinance.Domain.Entities;

namespace MyFinance.Infrastructure.Persistence;

/// <summary>
/// DbContext de escrita (também é o <see cref="IUnitOfWork"/>). Leituras quentes
/// (dashboard/resumo/painel de dívida) usam Dapper, não este contexto.
/// </summary>
public class MyFinanceDbContext(DbContextOptions<MyFinanceDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyFinanceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
