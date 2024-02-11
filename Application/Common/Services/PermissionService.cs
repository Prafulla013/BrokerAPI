using Application.Common.Interfaces;
using Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        public PermissionService(IBrokerDbContext dbContext,
                                 IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public async Task<bool> IsValidBroker(long brokerId, string subdomain, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Brokers.AnyAsync(a => a.Id == brokerId ,
                                                                        cancellationToken);
            return result;
        }
        public async Task<bool> HasPermissionAsync(long brokerId,
                                                   string subdomain,
                                                   string userId,
                                                   string[] roles,
                                                   UserType[] employeeTypes,
                                                   ModuleGroup moduleGroup,
                                                   ModuleAction moduleAction,
                                                   CancellationToken cancellationToken)
        {
            var isValid = await IsValidBroker(brokerId, subdomain, cancellationToken);
            if (!isValid)
                return false;

            var userProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                           .Where(w => w.Id == userId && w.BrokerId == brokerId)
                                                           .AsNoTracking()
                                                           .FirstOrDefaultAsync(cancellationToken);
            if (userProfile == null)
                return false;

            var userRoles = await _identityService.GetUserRolesAsync(userProfile.User);
            if (roles.Any(a => !userRoles.Contains(a)))
                return false;

            var identityRoles = await _identityService.ListRolesAsync(cancellationToken);

            var priorityRoles = identityRoles.Where(w => roles.Contains(w.Name)).Select(s => new { s.Id, s.Priority });
            var highestPriority = priorityRoles.Min(m => m.Priority);

            var hasPermission = await _dbContext.ModuleActionRolePermissions.Include(i => i.Role)
                                                                            .AnyAsync(a => (priorityRoles.Select(s => s.Id).Contains(a.RoleId) &&
                                                                                           (a.Role.Priority <= highestPriority || employeeTypes.Length == 0 || employeeTypes.Contains(a.UserType))) &&
                                                                                           a.ModuleGroup == moduleGroup &&
                                                                                           a.ModuleAction == moduleAction &&
                                                                                           a.IsActive == true,
                                                                                           cancellationToken);

            return hasPermission;
        }

        public async Task<bool> HasPermissionAsync(string userId,
                                                   string[] roles,
                                                   ModuleGroup moduleGroup,
                                                   ModuleAction moduleAction,
                                                   CancellationToken cancellationToken)
        {
            var userProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                           .Where(w => w.Id == userId && w.BrokerId == null)
                                                           .AsNoTracking()
                                                           .FirstOrDefaultAsync(cancellationToken);
            if (userProfile == null)
                return false;

            var userRoles = await _identityService.GetUserRolesAsync(userProfile.User);
            if (roles.Any(a => !userRoles.Contains(a)))
                return false;

            var identityRoles = await _identityService.ListRolesAsync(cancellationToken);
            var rolesIds = identityRoles.Where(w => roles.Contains(w.Name)).Select(s => s.Id);

            var hasPermission = await _dbContext.ModuleActionRolePermissions.AnyAsync(a => rolesIds.Contains(a.RoleId) &&
                                                                                           a.ModuleGroup == moduleGroup &&
                                                                                           a.ModuleAction == moduleAction &&
                                                                                           a.IsActive == true &&
                                                                                           a.UserType == 0,
                                                                                           cancellationToken);

            return hasPermission;
        }
    }
}
