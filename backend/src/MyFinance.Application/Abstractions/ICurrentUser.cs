namespace MyFinance.Application.Abstractions;

/// <summary>
/// Usuário autenticado na requisição atual. Base do isolamento multi-tenant:
/// toda consulta/mutação é escopada por <see cref="UserId"/> (RN13).
/// </summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
