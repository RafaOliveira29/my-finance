namespace MyFinance.Domain.Abstractions;

/// <summary>Base de toda entidade do domínio: identidade por <see cref="Id"/> (não por valor).</summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id) => Id = id;

    /// <summary>Construtor sem parâmetros para o materializador do EF Core.</summary>
    protected Entity() { }

    public override bool Equals(object? obj) =>
        obj is Entity other
        && GetType() == other.GetType()
        && Id != Guid.Empty
        && Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}

/// <summary>Raiz de agregado — fronteira de consistência transacional (ver 05-modelagem).</summary>
public abstract class AggregateRoot : Entity
{
    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() { }
}
