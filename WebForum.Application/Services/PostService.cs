using WebForum.Application.Converters;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;

namespace WebForum.Application.Services;

public class PostService(IPostRepository postRepository) : IPostService
{
    public async Task<IEnumerable<PostDto>> GetPostsAsync()
    {
        var posts = await postRepository.GetPostsAsync().ConfigureAwait(false);
        return posts.Select(PostConverter.MapToDto);
    }

    public async Task<PostDto> CreatePostAsync(string title, string body, Guid userId)
    {
        var post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createdPost = await postRepository.CreatePostAsync(post).ConfigureAwait(false);
        return PostConverter.MapToDto(createdPost);
    }
}
