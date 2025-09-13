using Microsoft.AspNetCore.Mvc;
using WebForum.Application.DTOs;
using WebForum.Application.Interfaces;

namespace WebForum.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="request">User registration information</param>
    /// <returns>The newly created <see cref="UserDto"/></returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await userService.RegisterAsync(request).ConfigureAwait(false);
        return Created(string.Empty, response);
    }
}
