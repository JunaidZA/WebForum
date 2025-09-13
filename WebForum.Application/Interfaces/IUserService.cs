using WebForum.Application.DTOs;

namespace WebForum.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterRequest request);
}
