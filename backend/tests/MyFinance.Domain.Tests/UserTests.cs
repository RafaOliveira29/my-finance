using MyFinance.Domain.Entities;
using Shouldly;

namespace MyFinance.Domain.Tests;

public class UserTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_valido_normaliza_email_e_seta_timestamps()
    {
        var user = User.Create("  Rafael ", "  Rafael@Example.COM ", "hash", Now);

        user.Name.ShouldBe("Rafael");
        user.Email.ShouldBe("rafael@example.com");
        user.PasswordHash.ShouldBe("hash");
        user.CreatedAt.ShouldBe(Now);
        user.UpdatedAt.ShouldBe(Now);
        user.Id.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("", "a@b.com", "hash")]
    [InlineData("Nome", "", "hash")]
    [InlineData("Nome", "a@b.com", "")]
    public void Create_invalido_lanca(string name, string email, string passwordHash)
    {
        Should.Throw<ArgumentException>(() => User.Create(name, email, passwordHash, Now));
    }

    [Fact]
    public void ChangePassword_atualiza_hash_e_updatedAt()
    {
        var user = User.Create("Rafael", "a@b.com", "old", Now);
        var later = Now.AddDays(1);

        user.ChangePassword("new", later);

        user.PasswordHash.ShouldBe("new");
        user.UpdatedAt.ShouldBe(later);
    }
}
