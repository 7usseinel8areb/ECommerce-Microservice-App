using AuthenticationApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApi.Infrastructure.Services;

internal class PasswordService : IPasswordService
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
    public string HashPasswordByBCrypt(string password)
    {
        // The WorkFactor controls computational cost (default 10)
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPasswordByBCrypt(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
