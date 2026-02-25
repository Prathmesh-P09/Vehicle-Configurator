using Microsoft.AspNetCore.Mvc;
using project_vc_.DTOs;
using project_vc_.Services;

namespace project_vc_.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = _service.Login(request);
            return Ok(token); // Java returned simple string token? line 29: return ResponseEntity.ok(token); Yes.
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
