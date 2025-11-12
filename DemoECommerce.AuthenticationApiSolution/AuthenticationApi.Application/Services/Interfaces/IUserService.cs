using AuthenticationApi.Application.DTOs;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Services.Interfaces;

public interface IUserService
{
    Task<Response> RegisterAsync(AppUserDTO userDto);
    Task<Response> LoginAsync(LoginDTO loginDto);
    Task<GetUserDTO> GetUserAsync(int id);
}
