using Microsoft.EntityFrameworkCore;

namespace MyFinance.Infrastructure.Persistence;

/// <summary>
/// DbContext de escrita. As entidades e suas <c>IEntityTypeConfiguration</c> entram
/// nas próximas fases; aqui já ficam prontos o assembly-scan das configurações e a base.
/// Leituras quentes (dashboard/resumo/painel de dívida) usam Dapper, não este contexto.
/// </summary>
public class MyFinanceDbContext(DbContextOptions<MyFinanceDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyFinanceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
