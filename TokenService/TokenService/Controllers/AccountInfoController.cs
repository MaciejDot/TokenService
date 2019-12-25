using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TokenService.Models;
namespace TokenService.Controllers

{
    [EnableCors]
    [Route("AccountInfo")]
    [ApiController]
    [AllowAnonymous]
    public class AccountInfoController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public ActionResult<UserInfoDTO> Get()
        {
            return new UserInfoDTO{ Username = User.Claims.Single(x=>x.Type=="Name").Value };
        }
    }
}