using server.Dto;
using server.Models;

namespace server.Interfaces
{
    public interface IAuthRepository
    {
        Task<UserDto> LoginAsync(UserDto authUserDto);
        Task<UserDto> RegisterAsync(UserDto authUserDto);
    }
}