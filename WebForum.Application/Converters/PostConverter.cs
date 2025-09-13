using WebForum.Application.DTOs;
using WebForum.Domain.Entities;

namespace WebForum.Application.Converters;

public static class PostConverter
{
    public static PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            Likes = post.LikeCount,
            CreatedAtUtc = post.CreatedAtUtc,
            Author = post.User.Username,
            Comments = post.Comments?.Select(CommentConverter.MapToDto).ToList() ?? [],
            Tags = post.Tags?.Select(TagConverter.MapToDto).ToList() ?? []
        };
    }
}
