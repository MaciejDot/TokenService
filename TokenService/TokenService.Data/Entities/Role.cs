using System;
using System.Collections.Generic;
using System.Text;

namespace TokenService.Data.Entities
{
    public class Role
    {
        public string Name { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
