using project_vc_.Models;
using project_vc_.Data;
using project_vc_.Utils;

namespace project_vc_.Services;

using Microsoft.EntityFrameworkCore;

    public interface IUserService
    {
        List<User> GetAllRegistrations();
        User SaveRegistration(User user);
        User GetById(int id);
        Task<User?> GetByIdentityAsync(string identifier);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public UserService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public List<User> GetAllRegistrations()
        {
            return _context.Users.ToList();
        }

        public User SaveRegistration(User user)
        {
            // 1. Validation
            if (_context.Users.Any(u => u.Email == user.Email))
                throw new Exception("Email already registered");
                
            if (_context.Users.Any(u => u.Username == user.Username))
                throw new Exception("Username already exists");
                
            if (!string.IsNullOrEmpty(user.Phone) && _context.Users.Any(u => u.Phone == user.Phone))
                throw new Exception("Phone number already registered");

            // 2. Logic
            string regNo = "VCONF-" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            user.RegistrationNo = regNo;
            user.Role = "USER";
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password); // Hash password!

            _context.Users.Add(user);
            _context.SaveChanges();

            // 3. Post-Save
            byte[] pdfBytes = PdfGeneratorUtil.GenerateUserPdf(user);
            
       
            _emailService.SendRegistrationEmail(user, pdfBytes);

            return user;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id) ?? throw new Exception("User not found");
        }

        public async Task<User?> GetByIdentityAsync(string identifier)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == identifier || u.Email == identifier);
        }
    }
