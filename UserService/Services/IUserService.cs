using UserService.Models;

namespace UserService.Services;
public interface IUserService
    {
        List<User> GetAllUsers();
        Task<User> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        void UpdateUser(int id, User updatedUser);
        Task DeleteUserAsync(int id);
        Task<User> DisableUserAsync(int userId);
        void StartBackgroundSync();
    }