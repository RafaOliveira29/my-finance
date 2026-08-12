namespace MyFinance.Application.Abstractions;

/// <summary>Hashing de senha (Argon2id na infraestrutura). O domínio só guarda o hash.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string hash, string password);
}
