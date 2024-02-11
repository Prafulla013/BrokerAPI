using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class AccountLoginHandler : IRequestHandler<AccountLoginCommand, AccountLoginResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IModuleActionPermissions _moduleActionPermissions;
        public AccountLoginHandler(IBrokerDbContext dbContext,
                                   IModuleActionPermissions actionPermissions,
                                   IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _moduleActionPermissions = actionPermissions;
        }

        public async Task<AccountLoginResponse> Handle(AccountLoginCommand request, CancellationToken cancellationToken)
        {
            long? brokerId = null;
            string brokerName = "";

            var dbUser = await _identityService.GetByNameAsync(request.Username, cancellationToken);
            if (dbUser == null)
            {
                throw new BadRequestException("Invalid username or password.");
            }

            dbUser.LastAccessedAt = DateTimeOffset.UtcNow;
            await _identityService.UpdateAsync(dbUser);

            await _identityService.CheckPasswordAsync(dbUser, request.Password);
            var token = await _identityService.GenerateTokenAsync(dbUser);

            var roles = await _identityService.GetUserRolesAsync(dbUser);

            List<UserType> employeeTypes = new List<UserType>();

            var response = new AccountLoginResponse
            {
                FirstName = dbUser.Profile.FirstName,
                LastName = dbUser.Profile.LastName,
                Broker = brokerName,
                Roles = roles,
                ModuleActionPermissions = await _moduleActionPermissions.GetModuleActionPermissionsAsync(roles, employeeTypes, cancellationToken),
                Token = token
            };
            return response;
        }

    }

    public class AccountLoginCommand : IRequest<AccountLoginResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string Subdomain { get; set; }
    }

    public class AccountLoginResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Broker { get; set; }
        public IList<string> Roles { get; set; }
        public IList<ListModulePermissionResponse> ModuleActionPermissions { get; set; }
        public string Token { get; set; }
    }

    public class ListModulePermissionResponse
    {
        public ModuleAction ModuleAction { get; set; }
        public ModuleGroup ModuleGroup { get; set; }
    }
}
