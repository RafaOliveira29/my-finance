using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Infrastructure.Persistence;

namespace MyFinance.Infrastructure;

/// <summary>Composition root da infraestrutura (persistência EF Core + Npgsql).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' não configurada (appsettings ou env ConnectionStrings__Default).");

        services.AddDbContext<MyFinanceDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
