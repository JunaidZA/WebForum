using Microsoft.AspNetCore.Mvc;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;
using WebForum.Domain.Entities;

namespace WebForum.Api.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController(IPostService postService) : ControllerBase
{
    /// <summary>
    /// Gets all posts.
    /// </summary>
    /// <returns>A list of <see cref="PostDto"/></returns>
    [HttpGet(Name = "GetPosts")]
    [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostsAsync() //TODO: Add filters etc
    {
        var posts = await postService.GetPostsAsync().ConfigureAwait(false);
        return Ok(posts);
    }

    /// <summary>
    /// Creates a new post.
    /// </summary>
    /// <returns>The created <see cref="Post"/></returns>
    [HttpPost(Name = "CreatePost")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostRequest request)
    {
        var post = await postService.CreatePostAsync(request.Title, request.Body, request.UserId).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Adds a comment to a post.
    /// </summary>
    /// <param name="postId">The ID of the post to add a comment to.</param>
    /// <returns></returns>
    [HttpPost("{postId:guid}/comments", Name = "AddComment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddCommentToPostAsync([FromRoute] Guid postId)
    {
        return NoContent();
    }

    /// <summary>
    /// Adds a like to a post.
    /// </summary>
    /// <param name="postId">The ID of the post to add a like to.</param>
    /// <returns></returns>
    [HttpPost("{postId:guid}/likes", Name = "AddLike")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddLikeToPostAsync([FromRoute] Guid postId)
    {
        return NoContent();
    }

    /// <summary>
    /// Removes a like from a post.
    /// </summary>
    /// <param name="postId">The ID of the post to remove a like from.</param>
    /// <returns>No content</returns>
    [HttpDelete("{postId:guid}/likes", Name = "RemoveLike")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveLikeFromPostAsync([FromRoute] Guid postId)
    {
        return NoContent();
    }

    /// <summary>
    /// Adds a like to a post.
    /// Moderator action.
    /// </summary>
    /// <param name="postId">The ID of the post to add a tag to.</param>
    /// <returns></returns>
    [HttpPost("{postId:guid}/tags", Name = "AddTag")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddTagToPostAsync([FromRoute] Guid postId)
    {
        return NoContent();
    }
}
