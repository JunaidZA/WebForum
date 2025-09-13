namespace WebForum.Application.DTOs;

public record PostDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public int Likes { get; set; }

    public string Author { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public ICollection<TagDto> Tags { get; init; } = new List<TagDto>();

    public ICollection<CommentDto> Comments { get; init; } = new List<CommentDto>();
}
