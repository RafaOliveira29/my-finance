using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyFinance.Infrastructure.Persistence;

/// <summary>
/// Fábrica de design-time: permite <c>dotnet ef migrations add ...</c> sem subir a API.
/// A string de conexão vem da env <c>MYFINANCE_DB</c> ou usa o padrão do docker-compose local.
/// </summary>
public sealed class MyFinanceDbContextFactory : IDesignTimeDbContextFactory<MyFinanceDbContext>
{
    public MyFinanceDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("MYFINANCE_DB")
            ?? "Host=localhost;Port=5433;Database=myfinance;Username=myfinance;Password=myfinance_dev";

        // Mesmas convenções do runtime (snake_case) para a migration bater com o schema real.
        var options = new DbContextOptionsBuilder<MyFinanceDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new MyFinanceDbContext(options);
    }
}
