using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Application.Common;

namespace MyFinance.API.Middleware;

/// <summary>
/// Converte qualquer exceção em <c>ProblemDetails</c> padronizado (CA071 / HT05):
/// falha de validação (FluentValidation) vira 400 com o dicionário de erros;
/// <see cref="AppException"/> vira 400/401/404/409 com título de negócio;
/// o resto vira 500 logado, sem vazar detalhes internos.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int status;
        string title;
        IDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case FluentValidation.ValidationException validation:
                status = StatusCodes.Status400BadRequest;
                title = "Requisição inválida";
                errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            case AppException app:
                status = app.StatusCode;
                title = app.Title;
                break;

            default:
                status = StatusCodes.Status500InternalServerError;
                title = "Erro interno";
                break;
        }

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado ao processar {Path}", httpContext.Request.Path);

        httpContext.Response.StatusCode = status;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Type = $"https://httpstatuses.io/{status}",
        };
        if (errors is not null)
            problem.Extensions["errors"] = errors;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problem,
        });
    }
}
