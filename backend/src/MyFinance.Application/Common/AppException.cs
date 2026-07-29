namespace MyFinance.Application.Common;

/// <summary>
/// Erro de negócio previsível. O handler global da API o converte em <c>ProblemDetails</c>
/// com o <see cref="StatusCode"/> e o <see cref="Title"/> corretos (CA071 / HT05).
/// Não depende de ASP.NET — a camada Application é livre de framework.
/// </summary>
public abstract class AppException(string message) : Exception(message)
{
    public abstract string Title { get; }
    public abstract int StatusCode { get; }
}

/// <summary>Recurso inexistente ou fora do escopo do usuário → 404.</summary>
public sealed class NotFoundException(string message) : AppException(message)
{
    public override string Title => "Recurso não encontrado";
    public override int StatusCode => 404;
}

/// <summary>Entrada inválida / violação de regra de negócio → 400.</summary>
public sealed class DomainValidationException(string message) : AppException(message)
{
    public override string Title => "Requisição inválida";
    public override int StatusCode => 400;
}

/// <summary>Conflito de estado (ex.: duplicidade, superpagamento) → 409.</summary>
public sealed class ConflictException(string message) : AppException(message)
{
    public override string Title => "Conflito de estado";
    public override int StatusCode => 409;
}
