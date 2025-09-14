namespace WebForum.Application.DTOs;

public record PagedResult<T>
{
    /// <summary>
    /// The items for the current page
    /// </summary>
    public required IEnumerable<T> Items { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items
    /// </summary>
    public long TotalCount { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}