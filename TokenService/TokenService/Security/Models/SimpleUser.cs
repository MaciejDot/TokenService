using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenService.Security.Models
{
    public class SimpleUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
    }
}
