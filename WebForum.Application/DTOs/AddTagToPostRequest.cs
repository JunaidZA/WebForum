using System.ComponentModel.DataAnnotations;

namespace WebForum.Application.DTOs;

public record AddTagToPostRequest
{
    [Required]
    [MaxLength(200)]
    public required string TagName { get; init; }
}
