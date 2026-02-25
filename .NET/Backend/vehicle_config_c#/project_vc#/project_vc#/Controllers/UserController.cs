using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_vc_.Services;
using System.Security.Claims;
using project_vc_.Models;

namespace project_vc_.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) 
        {
             // Log claims to see what happened
             var claims = User.Claims.Select(c => $"{c.Type}: {c.Value}");
             return Unauthorized(new { message = "Username claim not found", claims });
        }

        var user = await _service.GetByIdentityAsync(username);
        if (user == null) return NotFound(new { message = "User not found in DB", username });

        return Ok(new {
            user.AuthName,
            user.Email,
            user.Role,
            user.CompanyName,
            user.RegistrationNo
        });
    }

    [HttpGet("test")]
    [AllowAnonymous]
    public IActionResult Test()
    {
        return Ok("UserController routing is working correctly");
    }

    [HttpGet("anonymous-profile")]
    [AllowAnonymous]
    public IActionResult AnonymousProfile()
    {
        return Ok("AnonymousProfile route reached - routing is working for UserController");
    }
}
