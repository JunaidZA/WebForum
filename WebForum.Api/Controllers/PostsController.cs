using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;

namespace WebForum.Api.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController(IPostService postService) : ControllerBase
{
    /// <summary>
    /// Gets all posts with optional filtering, sorting, and paging support.
    /// </summary>
    /// <param name="postRequestFilter">Filter, pagination and sort parameters</param>
    /// <returns>A paged result of <see cref="PostDto"/></returns>
    [HttpGet(Name = "GetPosts")]
    [ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedResult<PostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPostsAsync([FromQuery] PostFilterRequest postFilterRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await postService.GetPostsAsync(postFilterRequest).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <returns>The created <see cref="Post"/></returns>
    [HttpPost(Name = "CreatePost")]
    [Authorize]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostRequest createPostRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
            }

            var post = await postService.CreatePostAsync(createPostRequest.Title, createPostRequest.Body, (Guid)userId).ConfigureAwait(false);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds a comment to a post.
    /// </summary>
    /// <param name="postId">The ID of the post to add a comment to.</param>
    /// <returns></returns>
    [HttpPost("{postId:guid}/comments", Name = "AddComment")]
    [Authorize]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCommentToPostAsync([FromRoute] Guid postId, [FromBody] CreateCommentRequest createCommentRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
            }
            await postService.AddCommentAsync(postId, createCommentRequest, (Guid)userId).ConfigureAwait(false);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds a like to a post.
    /// </summary>
    /// <param name="postId">The ID of the post to add a like to.</param>
    /// <returns></returns>
    [HttpPost("{postId:guid}/likes", Name = "AddLike")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddLikeToPostAsync([FromRoute] Guid postId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
            }
            await postService.AddLikeAsync(postId, (Guid)userId).ConfigureAwait(false);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Removes a like from a post.
    /// </summary>
    /// <param name="postId">The ID of the post to remove a like from.</param>
    /// <returns>No content</returns>
    [HttpDelete("{postId:guid}/likes", Name = "RemoveLike")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveLikeFromPostAsync([FromRoute] Guid postId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
            }
            await postService.RemoveLikeAsync(postId, (Guid)userId).ConfigureAwait(false);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Adds a like to a post.
    /// Moderator action.
    /// </summary>
    /// <param name="postId">The ID of the post to add a tag to.</param>
    /// <returns></returns>
    [HttpPost("{postId:guid}/tags", Name = "AddTag")]
    [Authorize("Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTagToPostAsync([FromRoute] Guid postId, [FromBody] AddTagToPostRequest addTagToPostRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await postService.AddTagToPostAsync(postId, addTagToPostRequest).ConfigureAwait(false);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        return null;
    }
}
