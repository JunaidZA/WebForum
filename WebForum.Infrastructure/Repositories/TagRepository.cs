using Microsoft.EntityFrameworkCore;
using WebForum.Domain.Entities;
using WebForum.Domain.Interfaces;
using WebForum.Infrastructure.Context;

namespace WebForum.Infrastructure.Repositories;

public class TagRepository(WebForumDbContext context) : ITagRepository
{
    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await context.Tags
            .FirstOrDefaultAsync(t => t.Name == name)
            .ConfigureAwait(false);
    }

    public async Task<Tag> CreateAsync(Tag tag)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return tag;
    }
}