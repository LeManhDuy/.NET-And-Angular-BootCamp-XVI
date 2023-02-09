using server.Dto;
using server.Models;

namespace server.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUser();
        Task<User> GetUser(int userId);
        string GenerateToken(UserDto userDto);
        User GetCurrentUser();
        bool UserExist(int userId);
        bool UserExist(string userName, int userId);
        Task<UserDto> LoginAsync(UserDto authUserDto);
        Task<UserDto> RegisterAsync(UserDto authUserDto);
        Task DeleteUser(User user);
        Task UpdateUser(User user);
        Task Logout(UserDto authUserDto);
    }
}