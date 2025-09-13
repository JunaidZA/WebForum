using Microsoft.EntityFrameworkCore;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;
using WebForum.Infrastructure.Context;

namespace WebForum.Infrastructure.Repositories;

public class PostRepository(WebForumDbContext context) : IPostRepository
{
    public async Task<IEnumerable<Post>> GetPostsAsync()
    {
        return await context.Posts
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return post;
    }
}
