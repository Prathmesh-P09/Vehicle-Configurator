namespace VehicleConfigurator.ConsoleApp.Utils
{
    public static class PasswordUtil
    {
        public static string HashPassword(string rawPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(rawPassword);
        }

        public static bool VerifyPassword(string rawPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(rawPassword, hashedPassword);
        }
    }
}
