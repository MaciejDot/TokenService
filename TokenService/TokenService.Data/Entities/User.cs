using System;
using System.Collections.Generic;
using System.Text;

namespace TokenService.Data.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
