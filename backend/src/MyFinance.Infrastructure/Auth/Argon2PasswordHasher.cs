using Isopoh.Cryptography.Argon2;
using MyFinance.Application.Abstractions;

namespace MyFinance.Infrastructure.Auth;

/// <summary>Hashing de senha com <b>Argon2id</b> (RNF04). O hash já embute salt e parâmetros.</summary>
public sealed class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => Argon2.Hash(password);

    public bool Verify(string hash, string password) => Argon2.Verify(hash, password);
}
