using VehicleConfigurator.ConsoleApp.DTOs;
using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Services
{
    public interface IUserService
    {
        Task<User> SaveRegistrationAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
