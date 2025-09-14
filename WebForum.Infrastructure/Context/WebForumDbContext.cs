using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebForum.Domain.Entities;

namespace WebForum.Infrastructure.Context;

public class WebForumDbContext : DbContext
{
    public WebForumDbContext(DbContextOptions<WebForumDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Post
        modelBuilder.Entity<Post>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Post>()
            .Property(b => b.Body)
            .HasMaxLength(10000);

        modelBuilder.Entity<Post>()
            .Property(b => b.Title)
            .HasMaxLength(100);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId);

        // User
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(t => t.Username)
            .HasColumnType("TEXT COLLATE NOCASE");

        // Comment
        modelBuilder.Entity<Comment>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId);

        // Like
        modelBuilder.Entity<Like>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId);

        // Tag
        modelBuilder.Entity<Tag>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<Tag>()
            .Property(t => t.Name)
            .HasColumnType("TEXT COLLATE NOCASE");


        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            // From: https://stackoverflow.com/questions/76149300/ef-core-can-not-handle-datetimeoffset-when-using-an-sqlite-connection
            // SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
            // here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
            // To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
            // use the DateTimeOffsetToBinaryConverter
            // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
            // This only supports millisecond precision, but should be sufficient for most use cases.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                            || p.PropertyType == typeof(DateTimeOffset?));
                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name)
                        .Property(property.Name)
                        .HasConversion(new DateTimeOffsetToBinaryConverter());
                }
            }
        }
    }
}
