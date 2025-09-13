using Microsoft.EntityFrameworkCore;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;
using WebForum.Infrastructure.Context;

namespace WebForum.Infrastructure.Repositories;

public class UserRepository(WebForumDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id)
            .ConfigureAwait(false);
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return user;
    }
}
