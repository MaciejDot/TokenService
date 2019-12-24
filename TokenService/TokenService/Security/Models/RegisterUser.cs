using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenService.Security.Models
{
    public class RegisterUser
    {
        public string Password { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
