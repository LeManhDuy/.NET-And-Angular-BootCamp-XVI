using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContext;
        public AuthRepository(DataContext context, IConfiguration config, IHttpContextAccessor httpContext)
        {
            _context = context;
            _config = config;
            _httpContext = httpContext;
        }

        public Task DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUser()
        {
            // var data = await _context.User
            //                             .Where(c => c.Hidden == false)
            //                             .OrderBy(c => c.Name)
            //                             .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            //                             .Take(validFilter.PageSize)
            //                             .ToListAsync();
            // return data;
            throw new NotImplementedException();
        }

        public Task<User> GetUser(int userId)
        {
            throw new NotImplementedException();
        }
        public Task<User> GetUser(string userName)
        {
            throw new NotImplementedException();
        }

        public Task Logout(UserDto authUserDto)
        {
            throw new NotImplementedException();
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

        public string GenerateToken(UserDto userDto)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var user = _context.Users.FirstOrDefault(p => p.Username == userDto.UserName);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["JWT:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public User GetCurrentUser()
        {
            var identity = _httpContext.HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                };
            }
            return null;
        }

        public Task UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool UserExist(int userId)
        {
            return _context.Users.Any(p => p.Id != userId);
        }

        public bool UserExist(string userName, int userId)
        {
            return _context.Users.Any(p => p.Id != userId && p.Username.ToLower() == userName.ToLower());
        }
        // public string Register(RegisterUserDto registerUserDto)
        // {
        //   throw new NotImplementedException();
        // }
    }
}