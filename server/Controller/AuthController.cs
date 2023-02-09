using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IAuthRepository _authRepository;
        private readonly DataContext _context;

        public AuthController(IAuthRepository authRepository, DataContext context)
        {
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

        ///// <summary>
        ///// hhhh
        ///// </summary>
        ///// <param name="userDto"></param>
        ///// <returns></returns>
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
                var token = await _authRepository.LoginAsync(userDto) != null ? _authRepository.GenerateToken(userDto) : null;
                return Ok(token);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}