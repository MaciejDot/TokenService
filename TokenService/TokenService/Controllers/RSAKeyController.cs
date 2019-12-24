using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenService.Security.Services;

namespace TokenService.Controllers
{
    [Route("/RSAKey")]
    [ApiController]
    public class RSAKeyController : ControllerBase
    {
        private readonly IUserService _userService;
        public RSAKeyController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<byte[]> Get()
        {
            return _userService.GetRSAPublicKey();
        }
    }
}