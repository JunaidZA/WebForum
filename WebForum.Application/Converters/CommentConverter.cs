using WebForum.Application.DTOs;
using WebForum.Domain.Entities;

namespace WebForum.Application.Converters;

public class CommentConverter
{
    public static CommentDto MapToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            Username = comment.User?.Username ?? string.Empty
        };
    }
}
