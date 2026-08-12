using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Application.Auth;

namespace MyFinance.Application;

/// <summary>Composition root da camada de aplicação (casos de uso e validadores).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos os validadores FluentValidation desta assembly.
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
