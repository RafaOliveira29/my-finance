using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MyFinance.Application;

/// <summary>Composition root da camada de aplicação (casos de uso e validadores).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos os validadores FluentValidation desta assembly (nenhum ainda na Fase 0).
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
