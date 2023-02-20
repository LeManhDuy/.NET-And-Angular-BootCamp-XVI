using api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using server.Data;
using server.Interfaces;

namespace api.Controller
{
    public class TokenController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly DataContext _context;

        public TokenController(ITokenRepository tokenRepository, DataContext context)
        {
            _tokenRepository = tokenRepository;
            _context = context;
        }

        //[HttpPost("tokens/cancel")]
        //public ActionResult CancelAccessToken([FromBody] string token)
        //{
        //    _tokenRepository.DestroyToken(token);
        //    return NoContent();
        //}
    }
}
