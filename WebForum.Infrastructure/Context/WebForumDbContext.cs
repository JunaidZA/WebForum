using Microsoft.EntityFrameworkCore;
using WebForum.Domain.Entities;

namespace WebForum.Infrastructure.Context;

public class WebForumDbContext : DbContext
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }
}
