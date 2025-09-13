using System.ComponentModel.DataAnnotations;

namespace WebForum.Application.DTOs;

public record CreatePostRequest
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; init; }

    [Required]
    [MaxLength(10000)]
    public required string Body { get; init; }

    public required Guid UserId { get; init; }
}
