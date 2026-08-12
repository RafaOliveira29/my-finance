namespace MyFinance.Application.Auth;

public sealed record RegisterRequest(string Name, string Email, string Password);

public sealed record LoginRequest(string Email, string Password);

public sealed record UserDto(Guid Id, string Name, string Email);

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, UserDto User);
