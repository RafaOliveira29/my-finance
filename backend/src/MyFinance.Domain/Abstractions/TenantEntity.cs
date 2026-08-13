namespace MyFinance.Domain.Abstractions;

/// <summary>
/// Entidade que pertence a um usuário (multi-tenant). O isolamento por <see cref="UserId"/> é
/// garantido pelo <c>EfRepository</c>, que escopa toda consulta pelo usuário atual (RN13) —
/// nenhuma consulta cruza usuários.
/// </summary>
public abstract class TenantEntity : AggregateRoot
{
    public Guid UserId { get; protected set; }

    protected TenantEntity(Guid id, Guid userId) : base(id)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId é obrigatório.", nameof(userId));
        UserId = userId;
    }

    protected TenantEntity() { } // EF Core
}