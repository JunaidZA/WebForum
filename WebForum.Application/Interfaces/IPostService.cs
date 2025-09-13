using WebForum.Application.DTOs;

namespace WebForum.Application.Interfaces;

public interface IPostService
{
    Task<IEnumerable<PostDto>> GetPostsAsync();
}
