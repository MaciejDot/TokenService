using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TokenService.Models;
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
        public ActionResult<RSAParametersDTO> Get()
        {
            var param = _userService.GetRSAPublicKey();
            return new RSAParametersDTO { 
                Modulus = param.Modulus,
                Exponent = param.Exponent
            };
        }
    }
}