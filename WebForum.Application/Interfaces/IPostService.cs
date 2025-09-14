using WebForum.Application.DTOs;

namespace WebForum.Application.Interfaces;

public interface IPostService
{
    /// <summary>
    /// Fetches all posts.
    /// </summary>
    /// <returns>A list of <see cref="PostDto"/></returns>
    Task<IEnumerable<PostDto>> GetPostsAsync();

    /// <summary>
    /// Fetches posts with filtering, sorting, and pagination.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<PagedResult<PostDto>> GetPostsAsync(PostFilterRequest filter);

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="userId"></param>
    /// <returns>The created <see cref="PostDto"/></returns>
    Task<PostDto> CreatePostAsync(string title, string body, Guid userId);

    /// <summary>
    /// Adds a comment to a post.
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task AddCommentAsync(Guid postId, CreateCommentRequest request, Guid userId);

    /// <summary>
    /// Adds a like to a post.
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task AddLikeAsync(Guid postId, Guid userId);

    /// <summary>
    /// Removes a like from a post.
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task RemoveLikeAsync(Guid postId, Guid userId);

    /// <summary>
    /// Adds a tag to a post.
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task AddTagToPostAsync(Guid postId, AddTagToPostRequest request);
}
