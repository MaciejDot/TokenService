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
    [Authorize]
    public class AccountInfoController : ControllerBase
    {
        [HttpGet]
        public ActionResult<UserInfoDTO> Get()
        {
            return new UserInfoDTO{ UserName = User.Claims.Single(x=>x.Type=="Name").Value };
        }
    }
}