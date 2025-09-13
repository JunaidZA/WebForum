using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;
using RegisterRequest = WebForum.Application.DTOs.RegisterRequest;

namespace WebForum.Application.Services;

public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
{
    private readonly string _secretKey = configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException(nameof(configuration), "JWT SecretKey not configured");
    private readonly string _issuer = configuration["JwtSettings:Issuer"] ?? "WebForum";
    private readonly string _audience = configuration["JwtSettings:Audience"] ?? "WebForum";
    private readonly int _expirationMinutes = int.Parse(configuration["JwtSettings:ExpirationMinutes"] ?? "60");

    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        var existingUserByEmail = await userRepository.GetByEmailAsync(request.Email).ConfigureAwait(false);
        if (existingUserByEmail != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var existingUserByUsername = await userRepository.GetByUsernameAsync(request.Username).ConfigureAwait(false);
        if (existingUserByUsername != null)
        {
            throw new InvalidOperationException("User with this username already exists");
        }

        var passwordHash = new PasswordHasher<object?>().HashPassword(null, request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
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

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email).ConfigureAwait(false);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var passwordVerificationResult = new PasswordHasher<object?>().VerifyHashedPassword(null, user.PasswordHash, request.Password);
        if (passwordVerificationResult != PasswordVerificationResult.Success)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = GenerateToken(user);
        return new LoginResponse
        {
            Token = token
        };
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.IsModerator ? "Moderator" : "User"),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
