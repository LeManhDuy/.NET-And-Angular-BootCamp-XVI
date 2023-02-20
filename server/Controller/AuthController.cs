using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Dto;
using server.Interfaces;

namespace server.Controller
{
    [Route("v1/api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IAuthRepository _authRepository;
        private readonly DataContext _context;

        public AuthController(ITokenRepository tokenRepository, IAuthRepository authRepository, DataContext context)
        {
            _tokenRepository = tokenRepository;
            _authRepository = authRepository;
            _context = context;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> RegisterAsync([FromBody] UserDto userDto)
        {
            if (userDto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authRepository.RegisterAsync(userDto);
                return Ok(userDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<UserDto>> LoginAsync([FromBody] UserDto userDto)
        {

            if (userDto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _authRepository.LoginAsync(userDto) != null ? _tokenRepository.GenerateToken(userDto) : null;
                setTokenCookie(token);
                return Ok(token);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        private void setTokenCookie(string token)
        {
            // append cookie with refresh token to the http response
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

    }
}