using Common.Enumerations;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Setup
{
    public static class SetupUser
    {
        public const string ID1 = "84a3bf8d-c65b-400a-968e-07c5a2601e85";
        public static string FirstName = "Test-1";
        public static string LastName = "User-1";
        public static string Email = "test-user@domain.com";
        public static string PhoneNumber = "9888000000";
        public static string Username = "test-user";
        public static string Password = "Test@2022";
        public static long? BrokerId = null;
        public static bool EmailConfirmed = true;
        public static bool IsActive = true;

        /// <summary>
        /// Not activated user
        /// </summary>
        public const string ID2 = "20de3903-0b29-4518-9e6b-97553fa4df40";
        public const string Username2 = "test-user2";

        public static async Task SeedUser(UserManager<User> userManager)
        {
            var profile1 = new UserProfile
            {
                Id = ID1,
                BrokerId = BrokerId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                HasSystemAccess = true,
                Type = UserType.RootUser,
                IsActive = true
            };

            var profile2 = new UserProfile
            {
                Id = ID2,
                BrokerId = BrokerId,
                FirstName = "Test-2",
                LastName = "User-2",
                Email = "test-user2@domain.com",
                PhoneNumber = "9977770000",
                HasSystemAccess = true,
                Type = UserType.RootUser,
                IsActive = false
            };

            var dbUsers = new List<User>
            {
                new User
                {
                    Id = ID1,
                    Email = Email,
                    UserName = Username,
                    EmailConfirmed = EmailConfirmed,
                    IsActive = IsActive,
                    Profile = profile1
                },
                new User
                {
                    Id = ID2,
                    Email = "test-user2@domain.com",
                    UserName = Username2,
                    EmailConfirmed = false,
                    IsActive = false,
                    Profile = profile2
                }
            };

            foreach (var dbUser in dbUsers)
            {
                await userManager.CreateAsync(dbUser);
                await userManager.AddPasswordAsync(dbUser, Password);
                // Internal/Application user role is always Super Admin
                await userManager.AddToRoleAsync(dbUser, "Super Admin");
            }
        }
    }
}
