using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Role : IdentityRole
    {
        public int Priority { get; set; }

        public virtual ICollection<UserRole> RoleUsers { get; set; }
    }
}
