using Application.Accounts.Commands;
using Application.Common.Interfaces;
using Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts.Services
{
    public class ModuleActionPermissions : IModuleActionPermissions
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        public ModuleActionPermissions(IIdentityService identityService, IBrokerDbContext dbContext)
        {
            _identityService = identityService;
            _dbContext = dbContext;
        }
        public async Task<List<ListModulePermissionResponse>> GetModuleActionPermissionsAsync(IList<string> roles, List<UserType> employeeTypes, CancellationToken cancellationToken)
        {
            var identityRoles = await _identityService.ListRolesAsync(cancellationToken);

            var brokerEmployee = identityRoles.Where(w => roles.Contains("Broker Employee") && w.Name == "Broker Employee").Select(s => s.Id).FirstOrDefault();

            var employeePermissions = new List<ListModulePermissionResponse>();
            if (employeeTypes.Any() && !string.IsNullOrEmpty(brokerEmployee))
            {
                employeePermissions = await _dbContext.ModuleActionRolePermissions.Where(w => employeeTypes.Contains(w.UserType) &&
                                                                                              w.RoleId == brokerEmployee && w.IsActive == true)
                                                                                  .Select(s => new ListModulePermissionResponse
                                                                                  {
                                                                                      ModuleAction = s.ModuleAction,
                                                                                      ModuleGroup = s.ModuleGroup
                                                                                  })
                                                                                  .Distinct()
                                                                                  .AsNoTracking()
                                                                                  .ToListAsync(cancellationToken);


            }

            var rolesIds = identityRoles.Where(w => roles.Where(w => w != "Broker Employee").Contains(w.Name)).Select(s => s.Id);
            var rolesPermissions = await _dbContext.ModuleActionRolePermissions.Where(w => rolesIds.Contains(w.RoleId) && w.IsActive == true)
                                                                               .Select(s => new ListModulePermissionResponse
                                                                               {
                                                                                   ModuleAction = s.ModuleAction,
                                                                                   ModuleGroup = s.ModuleGroup
                                                                               })
                                                                               .Distinct()
                                                                               .AsNoTracking()
                                                                               .ToListAsync(cancellationToken);

            var response = rolesPermissions.Union(employeePermissions).ToList();
            return response;
        }

    }
}
