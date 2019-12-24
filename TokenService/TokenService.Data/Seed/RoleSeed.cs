using System;
using System.Collections.Generic;
using System.Text;
using TokenService.Data.Entities;

namespace TokenService.Data.Seed
{
    public class RoleSeed
    {
        public static IEnumerable<Role> Roles() {
            yield return new Role { Name = "admin" };
        }
    }
}
