using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using TokenService.Models;
using TokenService.Security.Models;
using TokenService.Security.Services;

namespace TokenService.Controllers
{
    [EnableCors]
    [Route("Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        public RegisterController(IUserService userService) {
            _userService = userService;
        }
        [HttpPost]
        public async Task<ActionResult<TokenDTO>> Post([FromBody] RegisterUser user,CancellationToken token)
        {
            var emailRegexp = new Regex("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$");
            if(!(emailRegexp.IsMatch(user.Email) && emailRegexp.Match(user.Email).Value == user.Email))
            {
                return BadRequest();
            }
            if(await _userService.AddUser(user, token))
            {
                return new TokenDTO { Token = (await _userService.Authenticate(new AuthenticationModel { Email = user.Email, Password = user.Password }, token))};
            }
            return BadRequest();
        }
    }
}