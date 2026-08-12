using FluentValidation;
using MyFinance.Application.Abstractions;
using MyFinance.Application.Common;
using MyFinance.Domain.Entities;

namespace MyFinance.Application.Auth;

public sealed class AuthService(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);

        var email = User.NormalizeEmail(request.Email);
        if (await users.ExistsByEmailAsync(email, cancellationToken))
            throw new ConflictException("Já existe uma conta com este e-mail.");

        var user = User.Create(request.Name, email, passwordHasher.Hash(request.Password), DateTime.UtcNow);
        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var email = User.NormalizeEmail(request.Email);
        var user = await users.GetByEmailAsync(email, cancellationToken);

        // Mesma resposta para e-mail inexistente e senha errada (não vaza quais e-mails existem).
        if (user is null || !passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new UnauthorizedException("E-mail ou senha inválidos.");

        return BuildResponse(user);
    }

    private AuthResponse BuildResponse(User user)
    {
        var token = tokenService.Generate(user);
        return new AuthResponse(token.AccessToken, token.ExpiresAtUtc, new UserDto(user.Id, user.Name, user.Email));
    }
}
