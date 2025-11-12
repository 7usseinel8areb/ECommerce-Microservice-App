using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using eCommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces;

public interface IUser
{
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser> AddAsync(AppUser user);
}
