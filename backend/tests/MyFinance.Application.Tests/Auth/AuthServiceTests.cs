using MyFinance.Application.Auth;
using MyFinance.Application.Common;
using MyFinance.Domain.Entities;
using Shouldly;

namespace MyFinance.Application.Tests.Auth;

public class AuthServiceTests
{
    private static AuthService Build(FakeUserRepository repo, FakePasswordHasher? hasher = null) =>
        new(repo,
            new FakeUnitOfWork(),
            hasher ?? new FakePasswordHasher(),
            new FakeTokenService(),
            new RegisterRequestValidator(),
            new LoginRequestValidator());

    [Fact]
    public async Task Register_cria_usuario_com_senha_em_hash_e_retorna_token()
    {
        var repo = new FakeUserRepository();
        var sut = Build(repo);

        var response = await sut.RegisterAsync(new RegisterRequest("Rafael", "R@Ex.com", "password1"));

        response.AccessToken.ShouldNotBeNullOrEmpty();
        response.User.Email.ShouldBe("r@ex.com"); // normalizado
        repo.Saved.ShouldHaveSingleItem();
        repo.Saved[0].PasswordHash.ShouldNotBe("password1"); // nunca em texto puro
    }

    [Fact]
    public async Task Register_email_duplicado_lanca_conflict()
    {
        var repo = new FakeUserRepository();
        repo.Add(User.Create("X", "dup@ex.com", "h", DateTime.UtcNow));
        var sut = Build(repo);

        await Should.ThrowAsync<ConflictException>(() =>
            sut.RegisterAsync(new RegisterRequest("Y", "dup@ex.com", "password1")));
    }

    [Fact]
    public async Task Register_dados_invalidos_lanca_validation()
    {
        var sut = Build(new FakeUserRepository());

        await Should.ThrowAsync<FluentValidation.ValidationException>(() =>
            sut.RegisterAsync(new RegisterRequest("", "nao-e-email", "curta")));
    }

    [Fact]
    public async Task Login_correto_retorna_token()
    {
        var repo = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        repo.Add(User.Create("Rafael", "r@ex.com", hasher.Hash("password1"), DateTime.UtcNow));
        var sut = Build(repo, hasher);

        var response = await sut.LoginAsync(new LoginRequest("R@Ex.com", "password1"));

        response.AccessToken.ShouldNotBeNullOrEmpty();
        response.User.Email.ShouldBe("r@ex.com");
    }

    [Fact]
    public async Task Login_senha_errada_lanca_unauthorized()
    {
        var repo = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        repo.Add(User.Create("Rafael", "r@ex.com", hasher.Hash("password1"), DateTime.UtcNow));
        var sut = Build(repo, hasher);

        await Should.ThrowAsync<UnauthorizedException>(() =>
            sut.LoginAsync(new LoginRequest("r@ex.com", "senhaerrada")));
    }

    [Fact]
    public async Task Login_email_inexistente_lanca_unauthorized()
    {
        var sut = Build(new FakeUserRepository());

        await Should.ThrowAsync<UnauthorizedException>(() =>
            sut.LoginAsync(new LoginRequest("nao@existe.com", "password1")));
    }
}
