using Microsoft.AspNetCore.Identity.Data;
using WebForum.Application.DTOs;
using LoginRequest = WebForum.Application.DTOs.LoginRequest;
using RegisterRequest = WebForum.Application.DTOs.RegisterRequest;

namespace WebForum.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);
}
