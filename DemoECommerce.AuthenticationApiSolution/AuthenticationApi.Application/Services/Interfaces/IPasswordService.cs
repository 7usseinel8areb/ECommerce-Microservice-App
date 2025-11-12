namespace AuthenticationApi.Application.Services.Interfaces;

public interface IPasswordService
{
    string HashPassword(string password);
    string HashPasswordByBCrypt(string password);

    bool VerifyPassword(string password, string hashedPassword);
    bool VerifyPasswordByBCrypt(string password, string hashedPassword);
}
