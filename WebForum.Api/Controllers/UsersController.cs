using Microsoft.AspNetCore.Identity.Data;
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
    public async Task<IActionResult> RegisterAsync([FromBody] Application.DTOs.RegisterRequest request)
    {
        // Normally would not allow a user to set their own role during registration. But this is easier for testing.
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await userService.RegisterAsync(request).ConfigureAwait(false);
        return Created(string.Empty, response);
    }

    /// <summary>
    /// Authenticate user and return JWT token.
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>A JWT token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await userService.LoginAsync(request).ConfigureAwait(false);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
