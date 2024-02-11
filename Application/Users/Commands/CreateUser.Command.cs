using Application.Common.Interfaces;
using Common.Enumerations;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class CreateUseHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IStringHelper _stringHelper;
        private readonly UserManager<User> _userManager;
        public CreateUseHandler(IBrokerDbContext dbContext,
                                IIdentityService identityService,
                                IStringHelper stringHelper,
                                UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _stringHelper = stringHelper;
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var subdomain = "";
            var userType = UserType.RootUser;

            var dbUserProfile = new UserProfile
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                BrokerId = request.BrokerId,
                HasSystemAccess = true,
                Type = userType,
                IsActive = true,
                LastUpdatedBy = request.CurrentUser
            };


            var dbUser = new User
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Username,
                IsActive = false,
                Profile = dbUserProfile,
                LastUpdatedBy = request.CurrentUser,
                ClientUrl = _stringHelper.ToClientUrl(subdomain),
                Activity = ActivityLog.Created,
                UserRoles = new List<UserRole>()
            };
            foreach (var roleToAdd in request.Role)
            {
                var dbRole = await _identityService.GetRoleByNameAsync(roleToAdd);
                dbUser.UserRoles.Add(new UserRole
                {
                    RoleId = dbRole.Id
                });
            }

            await _identityService.CreateAsync(dbUser);

            var response = new CreateUserResponse
            {
                Id = dbUser.Id
            };
            return response;
        }
    }

    public class CreateUserCommand : IRequest<CreateUserResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public long? BrokerId { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
        public string[] Role { get; set; }
    }

    public class CreateUserResponse
    {
        public string Id { get; set; }
    }
}
