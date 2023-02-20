using server.Dto;
using server.Models;

namespace api.Interfaces
{
    public interface ITokenRepository
    {
        //int? DeactivateCurrentAsync();
        //int? DeactivateAsync(string token);
        string GenerateToken(UserDto userDto);
        User GetCurrentUser();
        bool IsTokenValid();
        //string DestroyToken(string token);
    }
}
