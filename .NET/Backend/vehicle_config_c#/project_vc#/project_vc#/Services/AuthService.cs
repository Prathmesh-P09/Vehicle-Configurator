using project_vc_.Models;
using project_vc_.DTOs;
using project_vc_.Data;
using BCrypt.Net;

namespace project_vc_.Services;

public interface IAuthService
{
    string Login(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtUtil _jwtUtil;

    public AuthService(ApplicationDbContext context, IJwtUtil jwtUtil)
    {
        _context = context;
        _jwtUtil = jwtUtil;
    }

    public string Login(LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null)
        {
            throw new Exception("Invalid username or password");
        }

        if (user.IsBlocked)
        {
            throw new Exception("User is blocked");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= 3)
            {
                user.IsBlocked = true;
            }
            _context.SaveChanges();
            throw new Exception("Invalid username or password");
        }

        user.FailedAttempts = 0;
        _context.SaveChanges();

        return _jwtUtil.GenerateToken(user.Username ?? user.Email);
    }
}
