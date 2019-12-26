using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenService.Models
{
    public class UserInfoDTO
    {
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
