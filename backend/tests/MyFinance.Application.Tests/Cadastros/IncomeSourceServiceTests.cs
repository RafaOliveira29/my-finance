using MyFinance.Application.Common;
using MyFinance.Application.Incomes;
using MyFinance.Domain.Entities;
using MyFinance.Domain.Enums;
using Shouldly;

namespace MyFinance.Application.Tests.Cadastros;

public class IncomeSourceServiceTests
{
    private static readonly Guid User = Guid.NewGuid();
    private static readonly DateOnly Start = new(2026, 1, 1);

    private static IncomeSourceService Build(FakeRepository<Category> cats, FakeRepository<IncomeSource> incs) =>
        new(incs, cats, new FakeUnitOfWork(), new FakeCurrentUser(User),
            new CreateIncomeSourceRequestValidator(), new UpdateIncomeSourceRequestValidator());

    [Fact]
    public async Task Create_com_categoria_inexistente_lanca_validation()
    {
        var svc = Build(new FakeRepository<Category>(), new FakeRepository<IncomeSource>());

        await Should.ThrowAsync<DomainValidationException>(() =>
            svc.CreateAsync(new CreateIncomeSourceRequest(Guid.NewGuid(), "Salário", 5000, 5, RecurrenceType.Monthly, Start, null, null)));
    }

    [Fact]
    public async Task Create_com_categoria_valida_persiste()
    {
        var cats = new FakeRepository<Category>();
        var incs = new FakeRepository<IncomeSource>();
        var cat = Category.Create(User, "Salário", CategoryType.Income, null, null, DateTime.UtcNow);
        cats.Add(cat);
        var svc = Build(cats, incs);

        var response = await svc.CreateAsync(
            new CreateIncomeSourceRequest(cat.Id, "Salário fixo", 5000, 5, RecurrenceType.Monthly, Start, null, null));

        response.Description.ShouldBe("Salário fixo");
        incs.Items.ShouldHaveSingleItem();
        incs.Items[0].UserId.ShouldBe(User);
    }
}