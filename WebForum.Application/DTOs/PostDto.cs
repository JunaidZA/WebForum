namespace WebForum.Application.DTOs;

public record PostDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public int Likes { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}
