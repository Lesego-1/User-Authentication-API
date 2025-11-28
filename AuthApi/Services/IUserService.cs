using AuthApi.DTOs;
using AuthApi.Models;

namespace AuthApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterRequest request);
        Task<bool> UserExistsAsync(string username, string email);
        Task<User?> AuthenticateAsync(LoginRequest request);

        // Add this line:
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByUsernameAsync(string username);

    }
}
