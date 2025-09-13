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
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .Include(p => p.Tags)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .Include(p => p.Tags)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id).ConfigureAwait(false);
    }

    public async Task<Post> CreatePostAsync(Post post)
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return post;
    }

    public async Task<Post> UpdatePostAsync(Post post)
    {
        context.Posts.Update(post);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return post;
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        context.Comments.Add(comment);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return comment;
    }

    public async Task<Like?> GetLikeAsync(Guid postId, Guid userId)
    {
        return await context.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId)
            .ConfigureAwait(false);
    }

    public async Task<Like> AddLikeAsync(Like like)
    {
        context.Likes.Add(like);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return like;
    }

    public async Task RemoveLikeAsync(Guid postId, Guid userId)
    {
        var like = await GetLikeAsync(postId, userId).ConfigureAwait(false);
        if (like != null)
        {
            context.Likes.Remove(like);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
