using WebForum.Domain.Entities;

namespace WebForum.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User> CreateAsync(User user);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByUsernameAsync(string username);
}
