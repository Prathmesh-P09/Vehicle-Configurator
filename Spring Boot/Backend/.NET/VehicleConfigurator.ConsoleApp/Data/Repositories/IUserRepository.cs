using VehicleConfigurator.ConsoleApp.Models;

namespace VehicleConfigurator.ConsoleApp.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindByUsernameAsync(string username);
        Task<User?> FindByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByPhoneAsync(string phone);
        Task<User> SaveAsync(User user);
        Task<List<User>> FindAllAsync();
        Task<User?> FindByIdAsync(int id);
    }
}
