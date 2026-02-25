using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace project_vc_.Services;

public class JwtUtil : IJwtUtil
{
    private readonly string _secretKey;

    public JwtUtil(IConfiguration config)
    {
        _secretKey = config["Jwt:Secret"] ?? "default_secret_key_long_enough";
    }

    public string GenerateToken(string username)
    {
        if (string.IsNullOrEmpty(username)) 
        {
            Console.WriteLine("[JWT GenerateToken] WARNING: Username is null or empty!");
            return string.Empty;
        }

        Console.WriteLine($"[JWT GenerateToken] Generating token for: {username}");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] 
            { 
                new Claim("sub", username),
                new Claim("unique_name", username),
                new Claim("name", username),
                new Claim("role", "USER") 
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "VehicleConfigAuth",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
