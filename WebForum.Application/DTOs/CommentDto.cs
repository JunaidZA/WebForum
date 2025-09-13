namespace WebForum.Application.DTOs;

public record CommentDto
{
    public Guid Id { get; init; }

    public string Body { get; init; }

    public Guid UserId { get; init; }

    public string Username { get; init; }
}
