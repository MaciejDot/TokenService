using System;
using System.Collections.Generic;
using System.Text;

namespace TokenService.Data.Entities
{
    public class UserRole
    {
        public string RoleId { get; set; }
        public Role Role { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
