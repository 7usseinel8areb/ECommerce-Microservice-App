using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Repositories;

internal class UserRepository(AuthenticationDbContext context) : IUser
{
    public async Task<AppUser?> GetByEmailAsync(string email)
         => await context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<AppUser?> GetByIdAsync(int id)
        => await context.Users.FindAsync(id);

    public async Task<AppUser> AddAsync(AppUser user)
    {
        var entry = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return entry.Entity;
    }
}
