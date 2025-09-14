using WebForum.Domain.Entities;

namespace WebForum.Domain.Interfaces;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetPostsAsync();

    Task<(IEnumerable<Post> Posts, long TotalCount)> GetPostsAsync(
        int page,
        int pageSize,
        string? author = null,
        IEnumerable<string>? tags = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        string sortBy = "CreatedDate",
        bool sortDescending = true);

    Task<Post?> GetPostByIdAsync(Guid id);

    Task<Post> CreatePostAsync(Post post);

    Task<Post> UpdatePostAsync(Post post);

    Task<Comment> AddCommentAsync(Comment comment);

    Task<Like?> GetLikeAsync(Guid postId, Guid userId);

    Task<Like> AddLikeAsync(Like like);

    Task RemoveLikeAsync(Guid postId, Guid userId);
}
