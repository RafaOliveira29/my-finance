using Microsoft.EntityFrameworkCore;
using MyFinance.Application.Abstractions;
using MyFinance.Domain.Entities;

namespace MyFinance.Infrastructure.Persistence;

/// <summary>
/// DbContext de escrita (também é o <see cref="IUnitOfWork"/>). O isolamento multi-tenant (RN13) é
/// aplicado no <c>EfRepository</c>, que escopa toda consulta pelo usuário atual — lido por requisição,
/// sem depender do modelo cacheado do EF. Leituras quentes (dashboard/resumo) usarão Dapper.
/// </summary>
public class MyFinanceDbContext(DbContextOptions<MyFinanceDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<IncomeSource> IncomeSources => Set<IncomeSource>();
    public DbSet<ExpenseSource> ExpenseSources => Set<ExpenseSource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyFinanceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}