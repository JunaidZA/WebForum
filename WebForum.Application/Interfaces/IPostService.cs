using WebForum.Application.DTOs;

namespace WebForum.Application.Interfaces;

public interface IPostService
{
    /// <summary>
    /// Fetches all posts.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<PostDto>> GetPostsAsync();

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<PostDto> CreatePostAsync(string title, string body, Guid userId);
}
