using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Mappers;
using AuthenticationApi.Application.Services.Interfaces;
using AuthenticationApi.Domain.Entities;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApi.Application.Services.Implementation;

internal class UserService : IUserService
{
    private readonly IUser _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public UserService(IUser userRepository, IPasswordService passwordService, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Response> RegisterAsync(AppUserDTO userDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
        if (existingUser is not null)
            return new Response { Flag = false, Message = "User with this email already exists." };

        var hashedPassword = _passwordService.HashPasswordByBCrypt(userDto.Password);

        var appUser = userDto.ToEntity();
        appUser.HashedPassword = hashedPassword;

        var addedUser = await _userRepository.AddAsync(appUser);

        return new Response
        {
            Flag = addedUser.Id > 0,
            Message = addedUser.Id > 0
                ? "User registered successfully."
                : "User registration failed."
        };
    }

    public async Task<Response> LoginAsync(LoginDTO loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
            return new Response { Flag = false, Message = "Invalid credentials." };

        var validPassword = _passwordService.VerifyPasswordByBCrypt(loginDto.Password, user.HashedPassword!);
        if (!validPassword)
            return new Response { Flag = false, Message = "Invalid credentials." };

        var token = _tokenService.GenerateToken(user.Id.ToString(), user.Email!, user.Role!);

        return new Response { Flag = true, Message = token };
    }

    public async Task<GetUserDTO> GetUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return null!;

        return user.ToGetDto();
    }
}
