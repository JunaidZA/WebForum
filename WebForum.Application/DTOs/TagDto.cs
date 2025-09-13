namespace WebForum.Application.DTOs;

public record TagDto
{
    public Guid Id { get; init; }

    public string Name { get; init; }
}