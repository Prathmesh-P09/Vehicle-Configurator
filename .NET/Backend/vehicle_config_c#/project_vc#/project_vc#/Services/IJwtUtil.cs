namespace project_vc_.Services;

public interface IJwtUtil
{
    string GenerateToken(string username);
}
