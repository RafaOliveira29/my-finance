using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Application.Auth;
using MyFinance.Application.Categories;
using MyFinance.Application.Expenses;
using MyFinance.Application.Incomes;

namespace MyFinance.Application;

/// <summary>Composition root da camada de aplicação (casos de uso e validadores).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra todos os validadores FluentValidation desta assembly.
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IIncomeSourceService, IncomeSourceService>();
        services.AddScoped<IExpenseSourceService, ExpenseSourceService>();

        return services;
    }
}
