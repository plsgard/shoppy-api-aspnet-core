using System;
using Microsoft.AspNetCore.Identity;

namespace Shoppy.Core.Roles
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Role(string roleName) : this()
        {
            Name = roleName;
        }
    }
}
