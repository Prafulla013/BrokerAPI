using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Setup
{
    public static class SetupRole
    {
        public static async Task SeedRole(RoleManager<Role> roleManager)
        {
            // Do not change this information as there will only be three roles, super admin, admin and employee
            var dbRoles = new List<Role> {
                new Role { Id = "24a1c6a6-0f83-42d4-b3e6-f9e5ab4c1026", Name = "Super Admin", NormalizedName = "SUPER ADMIN", Priority = 1, ConcurrencyStamp = "b517f2d9-c197-47ab-aa0c-fb4aa71137c0" },
                new Role { Id = "2fe8d0ce-c9ef-43aa-9a2f-e41e4d8d09f6", Name = "Admin", NormalizedName = "ADMIN", Priority = 2, ConcurrencyStamp = "99ed6629-547e-4f99-9b7e-b2b03412b724" },
                new Role { Id = "90da4fe1-3890-4c36-af89-90681a29a0ce", Name = "Employee", NormalizedName = "EMPLOYEE", Priority = 3, ConcurrencyStamp = "a2e68a42-f521-4f18-baa8-307de6c8e3b9" }
            };

            foreach (var dbRole in dbRoles)
            {
                await roleManager.CreateAsync(dbRole);
            }
        }
    }
}
