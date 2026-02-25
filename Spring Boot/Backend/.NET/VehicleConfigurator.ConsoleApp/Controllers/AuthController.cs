using Microsoft.AspNetCore.Mvc;
using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Services;

namespace VehicleConfigurator.ConsoleApp.Controllers
{
    [ApiController]
    [Route("auth")] // Matches /auth/login
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _userService.LoginAsync(dto);
                // The frontend might expect an object { token: "..." } or similar
                // Based on authService.js: API.post("/auth/login", data)
                // Returning a simple JSON logic
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }

    [ApiController]
    [Route("api/auth")] // Matches /api/auth/register
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var user = await _userService.SaveRegistrationAsync(dto);
                return Ok(new { message = "Registration successful", registrationNo = user.RegistrationNo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
