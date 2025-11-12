namespace AuthenticationApi.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(string userId, string email, string role);
    bool ValidateToken(string token);
}
