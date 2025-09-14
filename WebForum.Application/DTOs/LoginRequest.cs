namespace WebForum.Application.DTOs;

public record LoginRequest
{

    public string Email { get; init; }

    public string Password { get; init; }
}
