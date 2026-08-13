using MyFinance.Application.Categories;
using MyFinance.Application.Common;
using MyFinance.Domain.Entities;
using MyFinance.Domain.Enums;
using Shouldly;

namespace MyFinance.Application.Tests.Cadastros;

public class CategoryServiceTests
{
    private static readonly Guid User = Guid.NewGuid();
    private static readonly DateTime Now = DateTime.UtcNow;
    private static readonly DateOnly Start = new(2026, 1, 1);

    private static (CategoryService svc, FakeRepository<Category> cats, FakeRepository<IncomeSource> incs, FakeRepository<ExpenseSource> exps) Build()
    {
        var cats = new FakeRepository<Category>();
        var incs = new FakeRepository<IncomeSource>();
        var exps = new FakeRepository<ExpenseSource>();
        var svc = new CategoryService(cats, incs, exps, new FakeUnitOfWork(), new FakeCurrentUser(User),
            new CreateCategoryRequestValidator(), new UpdateCategoryRequestValidator());
        return (svc, cats, incs, exps);
    }

    [Fact]
    public async Task Create_persiste_com_usuario_atual()
    {
        var (svc, cats, _, _) = Build();

        var response = await svc.CreateAsync(new CreateCategoryRequest("Moradia", CategoryType.Expense, "#fff", "home"));

        response.Name.ShouldBe("Moradia");
        cats.Items.ShouldHaveSingleItem();
        cats.Items[0].UserId.ShouldBe(User);
    }

    [Fact]
    public async Task Create_invalido_lanca_validation()
    {
        var (svc, _, _, _) = Build();
        await Should.ThrowAsync<FluentValidation.ValidationException>(() =>
            svc.CreateAsync(new CreateCategoryRequest("", CategoryType.Expense, null, null)));
    }

    [Fact]
    public async Task Delete_categoria_em_uso_lanca_conflict()
    {
        var (svc, cats, incs, _) = Build();
        var cat = Category.Create(User, "Salário", CategoryType.Income, null, null, Now);
        cats.Add(cat);
        incs.Add(IncomeSource.Create(User, cat.Id, "Salário fixo", 5000, 5, RecurrenceType.Monthly, Start, null, null, Now));

        await Should.ThrowAsync<ConflictException>(() => svc.DeleteAsync(cat.Id));
    }

    [Fact]
    public async Task Delete_categoria_livre_remove()
    {
        var (svc, cats, _, _) = Build();
        var cat = Category.Create(User, "Lazer", CategoryType.Expense, null, null, Now);
        cats.Add(cat);

        await svc.DeleteAsync(cat.Id);

        cats.Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetById_inexistente_lanca_notfound()
    {
        var (svc, _, _, _) = Build();
        await Should.ThrowAsync<NotFoundException>(() => svc.GetByIdAsync(Guid.NewGuid()));
    }
}