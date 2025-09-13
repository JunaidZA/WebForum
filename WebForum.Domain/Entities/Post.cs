namespace WebForum.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public int LikeCount { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; }

    public ICollection<Tag> Tags { get; set; } = [];

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Like> Likes { get; set; } = [];
}
