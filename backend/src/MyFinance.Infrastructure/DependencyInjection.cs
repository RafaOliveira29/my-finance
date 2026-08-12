using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Application.Abstractions;
using MyFinance.Infrastructure.Auth;
using MyFinance.Infrastructure.Persistence;
using MyFinance.Infrastructure.Persistence.Repositories;

namespace MyFinance.Infrastructure;

/// <summary>Composition root da infraestrutura (persistência EF Core + Npgsql, auth).</summary>
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
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        // Unidade de trabalho = o próprio DbContext (mesma instância scoped do request).
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<MyFinanceDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
