using System.ComponentModel.DataAnnotations;

namespace WebForum.Application.DTOs;

public record RegisterRequest
{
    [Required]
    [MaxLength(100)]
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public required string Email { get; init; }

    [Required]
    [MinLength(6)]
    public required string Password { get; init; }

    public bool IsModerator { get; init; } = false;
}
