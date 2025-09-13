using Microsoft.AspNetCore.Identity;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;

namespace WebForum.Application.Services;

public class UserService(IUserRepository userRepository): IUserService
{
    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        var hashedPassword = new PasswordHasher<object?>().HashPassword(null, request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            IsModerator = request.IsModerator
        };

        var createdUser = await userRepository.CreateAsync(user).ConfigureAwait(false);
        return new UserDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = createdUser.Email,
            IsModerator = createdUser.IsModerator
        };
    }
}
