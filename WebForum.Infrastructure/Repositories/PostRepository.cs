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

    public async Task<(IEnumerable<Post> Posts, long TotalCount)> GetPostsAsync(
        int page,
        int pageSize,
        string? author = null,
        IEnumerable<string>? tags = null,
        DateTimeOffset? fromDate = null,
        DateTimeOffset? toDate = null,
        string sortBy = "CreatedDate",
        bool sortDescending = true)
    {
        var query = context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .Include(p => p.Tags)
            .Include(p => p.Likes)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(p => p.User != null && p.User.Username.ToLower().Contains(author.ToLower()));
        }

        if (tags != null && tags.Any())
        {
            var tagList = tags.ToList();
            query = query.Where(p => p.Tags.Any(t => tagList.Contains(t.Name)));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(p => p.CreatedAtUtc >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(p => p.CreatedAtUtc <= toDate.Value);
        }

        query = sortBy.ToLowerInvariant() switch
        {
            "likes" => sortDescending
                ? query.OrderByDescending(p => p.LikeCount).ThenByDescending(p => p.CreatedAtUtc)
                : query.OrderBy(p => p.LikeCount).ThenBy(p => p.CreatedAtUtc),
            "title" => sortDescending
                ? query.OrderByDescending(p => p.Title).ThenByDescending(p => p.CreatedAtUtc)
                : query.OrderBy(p => p.Title).ThenBy(p => p.CreatedAtUtc),
            _ => sortDescending
                ? query.OrderByDescending(p => p.CreatedAtUtc)
                : query.OrderBy(p => p.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync().ConfigureAwait(false);

        var posts = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync().ConfigureAwait(false);

        return (posts, totalCount);
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
