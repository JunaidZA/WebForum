using System.ComponentModel.DataAnnotations;

namespace WebForum.Application.DTOs;

public record CreateCommentRequest
{
    [Required]
    [MaxLength(10000)]
    public required string Body { get; init; }
}
