using VehicleConfigurator.ConsoleApp.Data.Repositories;
using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;
using VehicleConfigurator.ConsoleApp.Utils;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        
        // Simulating JWT generation
        private const string JwtSecret = "MySecretKeyForJwt12345678901234567890";

        public UserService(IUserRepository userRepo, IEmailService emailService)
        {
            _userRepo = userRepo;
            _emailService = emailService;
        }

        public async Task<User> SaveRegistrationAsync(RegisterDto dto)
        {
            // 1. Validation
            if (await _userRepo.ExistsByEmailAsync(dto.Email)) throw new Exception("Email already exists");
            if (await _userRepo.ExistsByUsernameAsync(dto.Username)) throw new Exception("Username already exists");

            // 2. Encryption (Explicit Aspect Logic)
            string encParams = PasswordUtil.HashPassword(dto.Password);

            // 3. Mapping
            var user = new User
            {
                Username = dto.Username,
                Password = encParams,
                Email = dto.Email,
                Role = "USER",
                Name = dto.AuthName, // Mapping AuthName to Name based on provided DTO usage
                AuthName = dto.AuthName,
                CompanyName = dto.CompanyName,
                CompanyStNo = dto.CompanyStNo,
                CompanyVatNo = dto.CompanyVatNo,
                Designation = dto.Designation,
                AuthTel = dto.AuthTel,
                Add1 = dto.Add1,
                City = dto.City,
                State = dto.State,
                RegistrationNo = "VCONF-" + DateTime.Now.Ticks
            };

            // 4. Persistence
            await _userRepo.SaveAsync(user);

            // 5. Side Effects (Email/PDF) - Simplified for Console context (Logging intent)
            // Ideally generate PDF here. For now, sending welcome email.
            string body = $"Welcome {user.Username}! Registration Successful. Your Reg ID: {user.RegistrationNo}";
            await _emailService.SendEmailAsync(user.Email, "Registration Confirmation", body);

            // 6. Logging (Audit)
            Console.WriteLine($"[AUDIT] User registered: {user.Username} at {DateTime.Now}");

            return user;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepo.FindByUsernameAsync(dto.Username);
            if (user == null) throw new Exception("User not found");

            if (user.IsBlocked) throw new Exception("User is blocked. Max attempts reached.");

            bool isValid = PasswordUtil.VerifyPassword(dto.Password, user.Password);

            if (!isValid)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 3)
                {
                    user.IsBlocked = true;
                    // Logging aspect hook
                    Console.WriteLine($"[AUDIT] User blocked due to invalid attempts: {user.Username}");
                }
                await _userRepo.SaveAsync(user);
                throw new Exception("Invalid Credentials");
            }

            // Success
            user.FailedAttempts = 0;
            await _userRepo.SaveAsync(user);
            Console.WriteLine($"[AUDIT] Login Success: {user.Username}");

            // Generate Token (Simulated)
            return $"TOKEN_{Guid.NewGuid()}_{user.Username}";
        }
    }
}
