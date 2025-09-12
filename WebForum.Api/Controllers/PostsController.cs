using Microsoft.AspNetCore.Mvc;

namespace WebForum.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    /// <summary>
    /// Gets all posts.
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetPosts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostsAsync()
    {
        return Ok();
    }
}
