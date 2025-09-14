using System.ComponentModel.DataAnnotations;

namespace WebForum.Application.DTOs;

public record PostFilterRequest
{
    /// <summary>
    /// Page number (1-based indexing)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filter by author username
    /// </summary>
    [MaxLength(100)]
    public string? Author { get; init; }

    /// <summary>
    /// Filter by tag names (comma-separated)
    /// </summary>
    public string? Tags { get; init; }

    /// <summary>
    /// Filter posts created from this date (inclusive)
    /// </summary>
    public DateTimeOffset? FromDate { get; init; }

    /// <summary>
    /// Filter posts created to this date (inclusive)
    /// </summary>
    public DateTimeOffset? ToDate { get; init; }

    /// <summary>
    /// Sort field options
    /// </summary>
    public PostSortField SortBy { get; init; } = PostSortField.CreatedDate;

    /// <summary>
    /// Sort direction
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Desc;
}

/// <summary>
/// Available sorting fields for posts
/// </summary>
public enum PostSortField
{
    CreatedDate,
    Likes
}

/// <summary>
/// Sort direction options
/// </summary>
public enum SortDirection
{
    Asc,
    Desc
}