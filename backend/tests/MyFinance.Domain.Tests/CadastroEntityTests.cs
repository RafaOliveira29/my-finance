using MyFinance.Domain.Entities;
using MyFinance.Domain.Enums;
using Shouldly;

namespace MyFinance.Domain.Tests;

public class CadastroEntityTests
{
    private static readonly Guid User = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateOnly Start = new(2026, 1, 1);

    [Fact]
    public void Category_create_valido()
    {
        var c = Category.Create(User, "  Moradia ", CategoryType.Expense, " #fff ", "home", Now);
        c.Name.ShouldBe("Moradia");
        c.Type.ShouldBe(CategoryType.Expense);
        c.Color.ShouldBe("#fff");
        c.IsActive.ShouldBeTrue();
        c.UserId.ShouldBe(User);
    }

    [Fact]
    public void Category_nome_vazio_lanca() =>
        Should.Throw<ArgumentException>(() => Category.Create(User, "  ", CategoryType.Income, null, null, Now));

    [Fact]
    public void TenantEntity_userId_vazio_lanca() =>
        Should.Throw<ArgumentException>(() => Category.Create(Guid.Empty, "X", CategoryType.Income, null, null, Now));

    [Theory]
    [InlineData(0)]
    [InlineData(32)]
    public void IncomeSource_competenceDay_fora_de_faixa_lanca(int day) =>
        Should.Throw<ArgumentException>(() =>
            IncomeSource.Create(User, Guid.NewGuid(), "Salário", 5000, day, RecurrenceType.Monthly, Start, null, null, Now));

    [Fact]
    public void IncomeSource_valor_negativo_lanca() =>
        Should.Throw<ArgumentException>(() =>
            IncomeSource.Create(User, Guid.NewGuid(), "Salário", -1, 5, RecurrenceType.Monthly, Start, null, null, Now));

    [Fact]
    public void ExpenseSource_create_valido()
    {
        var e = ExpenseSource.Create(User, Guid.NewGuid(), "Aluguel", ExpenseKind.Fixed, 1500, 10, 0, RecurrenceType.Monthly, Start, null, null, Now);
        e.ExpenseKind.ShouldBe(ExpenseKind.Fixed);
        e.DueDay.ShouldBe(10);
        e.DueMonthOffset.ShouldBe(0);
    }

    [Fact]
    public void ExpenseSource_dueDay_invalido_lanca() =>
        Should.Throw<ArgumentException>(() =>
            ExpenseSource.Create(User, Guid.NewGuid(), "Aluguel", ExpenseKind.Fixed, 1500, 40, 0, RecurrenceType.Monthly, Start, null, null, Now));

    [Fact]
    public void ExpenseSource_dueMonthOffset_invalido_lanca() =>
        Should.Throw<ArgumentException>(() =>
            ExpenseSource.Create(User, Guid.NewGuid(), "Aluguel", ExpenseKind.Fixed, 1500, 10, 2, RecurrenceType.Monthly, Start, null, null, Now));
}