using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System;
using TokenService.Security.Services;
using TokenService.Security.Models;
using TokenService.Models;

namespace TokenService.Controllers
{
    [EnableCors]
    [Route("Token")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;

        public TokenController(IUserService userService)
        {
            _userService= userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<TokenDTO>> Get(CancellationToken token)
        {
            var id = User.Claims.Single(x => x.Type == "Id").Value;
            return new TokenDTO { Token = (await _userService.GetTokenForUser(id, token))};
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<TokenDTO>> Post([FromBody]AuthenticationModel userModel, CancellationToken cancellationToken)
        {
            var token = string.Empty;
            try 
            {
                token = (await _userService.Authenticate(userModel, cancellationToken));
            }
            catch
            {
                return Unauthorized();
            }
            return new TokenDTO { Token = token };
        }
    }
}