using Common.Configurations;
using Common.Enumerations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence
{
    public class DbInitializer
    {
        private readonly ModelBuilder _modelBuilder;
        private readonly ApplicationConfiguration _applicationConfiguration;
        public DbInitializer(ModelBuilder modelBuilder,
                             ApplicationConfiguration applicationConfiguration)
        {
            _modelBuilder = modelBuilder;
            _applicationConfiguration = applicationConfiguration;
        }

        public const string SUPER_ADMIN_ID = "61181f2e-5502-4c50-be86-a446115ad73d";
        public const string ADMIN = "884b283c-f826-4eec-b287-ef020e568f08";
        public const string EMPLOYEE_ID = "c6be9b48-6a28-4401-b898-d44c6b662183";
        public void SeedUsersAndRoles()
        {
            var dbRoleSuperAdmin = new Role { Id = SUPER_ADMIN_ID, Name = "Super Admin", ConcurrencyStamp = "2d339c8e-171e-4391-b18c-59c9f664c97f", NormalizedName = "SUPER ADMIN", Priority = 1 };
            var dbRoleBrokerAdmin = new Role { Id = ADMIN, Name = "Broker Admin", ConcurrencyStamp = "4148e3f9-1504-46f7-b272-3f673a533da5", NormalizedName = "ADMIN", Priority = 3 };
            var dbRoleBrokerEmployee = new Role { Id = EMPLOYEE_ID, Name = "Broker Employee", ConcurrencyStamp = "64a63c1d-aca1-4246-a486-c06c174128cd", NormalizedName = "EMPLOYEE", Priority = 5 };


            _modelBuilder.Entity<Role>().HasData(dbRoleSuperAdmin, dbRoleBrokerAdmin, dbRoleBrokerEmployee);

            var dbProfile = new UserProfile
            {
                Id = "fb443e5c-c961-4a9a-9d87-11ec23b01f03",
                FirstName = "Master",
                LastName = "User",
                Email = "proshore@yopmail.com",
                PhoneNumber = "9000000000",
                HasSystemAccess = true,
                Type = UserType.RootUser,
                IsActive = true,
                CreatedAt = new DateTimeOffset(new DateTime(2023, 1, 11, 10, 10, 58, 959, DateTimeKind.Unspecified).AddTicks(6954), new TimeSpan(0, 0, 0, 0, 0)),
                LastUpdateDateTime = new DateTimeOffset(new DateTime(2023, 1, 11, 10, 10, 58, 959, DateTimeKind.Unspecified).AddTicks(6954), new TimeSpan(0, 0, 0, 0, 0))
            };
            _modelBuilder.Entity<UserProfile>().HasData(dbProfile);

            var dbUser = new User
            {
                Id = "fb443e5c-c961-4a9a-9d87-11ec23b01f03",
                UserName = "master",
                NormalizedUserName = "MASTER",
                Email = "proshore@yopmail.com",
                EmailConfirmed = true,
                NormalizedEmail = "proshore@yopmail.com".ToUpper(),
                IsActive = true,
                ConcurrencyStamp = "d3763a1e-5f21-4cca-b755-320f45862f7a",
                SecurityStamp = "fc1ceafe-eab6-4375-8a60-201ce8161831",
                LastUpdateDateTime = new DateTimeOffset(new DateTime(2023, 1, 11, 10, 10, 58, 959, DateTimeKind.Unspecified).AddTicks(6954), new TimeSpan(0, 0, 0, 0, 0)),
                LastAccessedAt = new DateTimeOffset(new DateTime(2023, 1, 11, 10, 10, 58, 959, DateTimeKind.Unspecified).AddTicks(6954), new TimeSpan(0, 0, 0, 0, 0))
            };
            _modelBuilder.Entity<User>().HasData(dbUser);

            var dbUserRole = new UserRole { UserId = dbUser.Id, RoleId = dbRoleSuperAdmin.Id };
            _modelBuilder.Entity<UserRole>().HasData(dbUserRole);
        }
    }
}
