using WebForum.Domain.Entities;

namespace WebForum.Domain.Interfaces;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetPostsAsync();
    Task<Post> CreatePostAsync(Post post);
}
