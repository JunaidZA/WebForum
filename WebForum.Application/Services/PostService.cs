using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;

namespace WebForum.Application.Services;

public class PostService() : IPostService
{
    public async Task<IEnumerable<PostDto>> GetPostsAsync()
    {
        return [];
    }
}
