using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using project_vc_.Services;
using System.Security.Claims;
using project_vc_.DTOs;

namespace project_vc_.Controllers
{
    [ApiController]
    [Route("oauth2/authorization")]
    public class OAuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtUtil _jwtUtil;

        public OAuthController(IUserService userService, IJwtUtil jwtUtil)
        {
            _userService = userService;
            _jwtUtil = jwtUtil;
        }

        [HttpGet("google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("ExternalCookie");
            if (!result.Succeeded)
                return BadRequest("Google authentication failed.");

            return await ProcessOAuthLogin(result.Principal);
        }

        [HttpGet("facebook")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("FacebookResponse") };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync("ExternalCookie");
            if (!result.Succeeded)
                return BadRequest("Facebook authentication failed.");

            return await ProcessOAuthLogin(result.Principal);
        }

        private async Task<IActionResult> ProcessOAuthLogin(ClaimsPrincipal principal)
        {
            var email = principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim not found from provider.");

            var user = await _userService.GetByIdentityAsync(email);

            if (user != null)
            {
                // User exists -> Generate Token & Redirect
                var token = _jwtUtil.GenerateToken(user.Email!);
                // Frontend URL: http://localhost:5173/oauth2/redirect?token=...
                return Redirect($"http://localhost:5173/oauth2/redirect?token={token}");
            }
            else
            {
                // User not found -> Redirect to Register with email
                // Frontend URL: http://localhost:5173/register?email=...
                return Redirect($"http://localhost:5173/register?email={email}");
            }
        }
    }
}
