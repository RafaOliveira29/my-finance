using MyFinance.Domain.Abstractions;
using Shouldly;

namespace MyFinance.Domain.Tests;

/// <summary>Smoke test da Fase 0: garante a identidade por Id da base do domínio.</summary>
public class EntityTests
{
    private sealed class Foo(Guid id) : Entity(id);
    private sealed class Bar(Guid id) : Entity(id);

    [Fact]
    public void Entidades_do_mesmo_tipo_com_mesmo_id_sao_iguais()
    {
        var id = Guid.NewGuid();
        new Foo(id).ShouldBe(new Foo(id));
    }

    [Fact]
    public void Tipos_diferentes_com_mesmo_id_nao_sao_iguais()
    {
        var id = Guid.NewGuid();
        new Foo(id).Equals(new Bar(id)).ShouldBeFalse();
    }

    [Fact]
    public void Id_vazio_nunca_e_igual()
    {
        new Foo(Guid.Empty).Equals(new Foo(Guid.Empty)).ShouldBeFalse();
    }
}
