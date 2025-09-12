using Microsoft.AspNetCore.Mvc;

namespace WebForum.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    [HttpGet(Name = "GetPosts")]
    public async Task<IActionResult> GetPostsAsync()
    {
        return Ok();
    }
}
