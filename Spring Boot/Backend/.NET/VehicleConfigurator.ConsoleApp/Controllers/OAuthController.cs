using Microsoft.AspNetCore.Mvc;

namespace VehicleConfigurator.ConsoleApp.Controllers
{
    [ApiController]
    [Route("oauth2")]
    public class OAuthController : ControllerBase
    {
        [HttpGet("authorization/google")]
        public IActionResult GoogleAuth()
        {
            // Real Google OAuth Redirect
            string clientId = "766477736814-apoh5ld8q9pgv41bmqpn4t0745pb4o0p.apps.googleusercontent.com"; // From application.yml
            string redirectUri = "http://localhost:8080/oauth2/redirect"; // Must match Google Console logic, typically backend handles code exchange
            string scope = "email profile";
            string googleUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope={scope}";
            
            // Note: If redirectUri mismatch error occurs from Google, usually means localhost:8080 is not whitelisted.
            // If so, user must whitelist it.
            
            return Redirect(googleUrl);
        }

        [HttpGet("redirect")]
        public IActionResult RedirectCallback(string code)
        {
            // Frontend calls this to exchange code for token
            return Ok(new { 
                token = "MOCK_JWT_TOKEN_" + Guid.NewGuid(),
                message = "OAuth Login Successful (Mock)"
            });
        }
    }
}
