using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using WebForum.Application.Services;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using LoginRequest = WebForum.Application.DTOs.LoginRequest;
using RegisterRequest = WebForum.Application.DTOs.RegisterRequest;

namespace WebForum.Tests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _configuration = Substitute.For<IConfiguration>();

        _configuration["JwtSettings:ExpirationMinutes"].Returns("60");
        _configuration["JwtSettings:SecretKey"].Returns("wasdasdasdsadasdasdasdasfasfasfasgasgasgasgasg");

        _userService = new UserService(_userRepository, _configuration);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "testemail@testdomain.co.jun",
            Password = "password123"
        };

        var passwordHash = new PasswordHasher<object?>().HashPassword(null, "password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "testemail@testdomain.co.jun",
            Username = "testuser",
            PasswordHash = passwordHash,
            IsModerator = false
        };

        _userRepository.GetByEmailAsync(loginRequest.Email).Returns(user);

        // Act
        var result = await _userService.LoginAsync(loginRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Token.ShouldNotBeNullOrEmpty();
        result.Token.Split('.').Length.ShouldBe(3);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@testdomain.co.jun",
            Password = "password123"
        };

        _userRepository.GetByEmailAsync(loginRequest.Email).Returns((User?)null);

        // Act
        // Assert
        var exception = await Should.ThrowAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(loginRequest));
        exception.Message.ShouldBe("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@testdomain.co.jun",
            Password = "wrongpassword"
        };

        var passwordHash = new PasswordHasher<object?>().HashPassword(null, "correctpassword");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@testdomain.co.jun",
            Username = "testuser",
            PasswordHash = passwordHash,
            IsModerator = false
        };

        _userRepository.GetByEmailAsync(loginRequest.Email).Returns(user);

        // Act
        // Assert
        var exception = await Should.ThrowAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(loginRequest));
        exception.Message.ShouldBe("Invalid email or password");
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ReturnsUserDto()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "newuser@testdomain.co.jun",
            Password = "password123",
            IsModerator = false
        };

        var passwordHash = new PasswordHasher<object?>().HashPassword(null, registerRequest.Password);
        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            PasswordHash = passwordHash,
            IsModerator = registerRequest.IsModerator
        };

        _userRepository.GetByEmailAsync(registerRequest.Email).Returns((User?)null);
        _userRepository.GetByUsernameAsync(registerRequest.Username).Returns((User?)null);
        _userRepository.CreateAsync(Arg.Any<User>()).Returns(createdUser);

        // Act
        var result = await _userService.RegisterAsync(registerRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdUser.Id);
        result.Username.ShouldBe(createdUser.Username);
        result.Email.ShouldBe(createdUser.Email);
        result.IsModerator.ShouldBe(createdUser.IsModerator);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "existing@testdomain.co.jun",
            Password = "password123",
            IsModerator = false
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "existinguser",
            Email = "existing@testdomain.co.jun",
            PasswordHash = "hash",
            IsModerator = false
        };

        _userRepository.GetByEmailAsync(registerRequest.Email).Returns(existingUser);

        // Act
        // Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _userService.RegisterAsync(registerRequest));
        exception.Message.ShouldBe("User with this email already exists");

        await _userRepository.DidNotReceive().CreateAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ThrowsInvalidOperationException()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "existinguser",
            Email = "new@testdomain.co.jun",
            Password = "password123",
            IsModerator = false
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "existinguser",
            Email = "existing@testdomain.co.jun",
            PasswordHash = "hash",
            IsModerator = false
        };

        _userRepository.GetByEmailAsync(registerRequest.Email).Returns((User?)null);
        _userRepository.GetByUsernameAsync(registerRequest.Username).Returns(existingUser);

        // Act
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => _userService.RegisterAsync(registerRequest));
        exception.Message.ShouldBe("User with this username already exists");

        await _userRepository.DidNotReceive().CreateAsync(Arg.Any<User>());
    }
}