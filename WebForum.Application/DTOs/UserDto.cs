namespace WebForum.Application.DTOs;

public record UserDto
{
    public Guid Id { get; init; }

    public string Username { get; init; }

    public string Email { get; init; }

    public bool IsModerator { get; init; }
}
