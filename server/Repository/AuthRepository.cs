using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using server.Data;
using server.Dto;
using server.Interfaces;
using server.Models;

namespace server.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserDto> LoginAsync(UserDto userDto)
        {
            try
            {
                userDto.UserName = userDto.UserName.ToLower();

                var currentUser = await _context.Users.Where(p => p.Username == userDto.UserName).FirstOrDefaultAsync();
                if (currentUser == null)
                    throw new Exception("User not found!");

                using var hmac = new HMACSHA512(currentUser.PasswordSalt);
                var passwordBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));
                for (int i = 0; i < currentUser.PasswordHashed.Length; i++)
                {
                    if (currentUser.PasswordHashed[i] != passwordBytes[i])
                    {
                        return null;
                    }
                }

                return userDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }

        public async Task<UserDto> RegisterAsync(UserDto userDto)
        {
            try
            {
                userDto.UserName = userDto.UserName.ToLower();

                var currentUser = await _context.Users.Where(p => p.Username == userDto.UserName).FirstOrDefaultAsync();
                if (currentUser != null)
                {
                    throw new Exception("Username is already exist!");
                }

                using var hmac = new HMACSHA512();
                var passwordBytes = Encoding.UTF8.GetBytes(userDto.Password);

                var newUser = new User
                {
                    Username = userDto.UserName,
                    PasswordHashed = hmac.ComputeHash(passwordBytes),
                    PasswordSalt = hmac.Key,
                    Role = userDto.Role
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                return userDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Message: " + ex.Message);
            }
        }
    }
}