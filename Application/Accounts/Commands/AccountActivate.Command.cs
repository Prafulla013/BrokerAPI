using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class AccountActivateHandler : IRequestHandler<AccountActivateCommand, AccountActivateResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IModuleActionPermissions _moduleActionPermissions;
        public AccountActivateHandler(IBrokerDbContext dbContext, IModuleActionPermissions actionPermissions, IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _moduleActionPermissions = actionPermissions;
        }

        public async Task<AccountActivateResponse> Handle(AccountActivateCommand request, CancellationToken cancellationToken)
        {
            long? brokerId = null;
            string brokerName = "";
           

            var dbProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                        .FirstOrDefaultAsync(fd => fd.Email.ToLower() == request.Email.ToLower(),
                                                                                    cancellationToken);
            if (dbProfile == null)
            {
                throw new BadRequestException("Invalid user request.");
            }

            var dbUser = dbProfile.User;
            dbUser.ClientUrl = request.ClientUrl;
            dbUser.IsActive = true;
            dbUser.LastUpdatedBy = $"{dbProfile.FirstName} {dbProfile.LastName}";
            dbUser.Activity = ActivityLog.Activated;

            await _identityService.ActivateAsync(dbUser, request.Token, request.Password);

            var token = await _identityService.GenerateTokenAsync(dbUser);
            var roles = await _identityService.GetUserRolesAsync(dbUser);

            List<UserType> employeeTypes = new List<UserType>();

            var response = new AccountActivateResponse
            {
                FirstName = dbProfile.FirstName,
                LastName = dbProfile.LastName,
                Broker = brokerName,
                Roles = roles,
                ModuleActionPermissions = await _moduleActionPermissions.GetModuleActionPermissionsAsync(roles, employeeTypes, cancellationToken),
                Token = token
            };
            return response;
        }
    }

    public class AccountActivateCommand : IRequest<AccountActivateResponse>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string Subdomain { get; set; }
        [JsonIgnore]
        public string ClientUrl { get; set; }
    }

    public class AccountActivateResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Broker { get; set; }
        public IList<string> Roles { get; set; }
        public IList<ListModulePermissionResponse> ModuleActionPermissions { get; set; }
        public string Token { get; set; }
    }

    public class ModulePermissionResponse
    {
        public ModuleAction ModuleAction { get; set; }
        public ModuleGroup ModuleGroup { get; set; }
    }
}
