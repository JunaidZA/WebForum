namespace WebForum.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public bool IsModerator { get; set; }

    public ICollection<Post> Posts { get; set; } = [];

    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Like> Likes { get; set; } = [];
}
