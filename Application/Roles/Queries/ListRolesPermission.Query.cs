using Application.Common.Interfaces;
using Common.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Roles.Queries
{
    public class ListRolesPermissionReportHandler : IRequestHandler<ListRolesPermissionQuery, List<ListRolesPermissionResponse>>
    {
        private readonly IBrokerDbContext _dbContext;
        public ListRolesPermissionReportHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListRolesPermissionResponse>> Handle(ListRolesPermissionQuery request, CancellationToken cancellationToken)
        {
            var superAdminId = "61181f2e-5502-4c50-be86-a446115ad73d";

            var response = await _dbContext.ModuleActionRolePermissions.Where(w => w.RoleId != superAdminId && w.IsActive == true)
                                                                       .Select(s => new
                                                                       {
                                                                           s.Role.Name,
                                                                           s.ModuleGroup,
                                                                           s.ModuleAction,
                                                                           s.UserType
                                                                       }).ToListAsync(cancellationToken);

            var rolePermission = new List<ListRolesPermissionResponse>();

            foreach (var roleGroup in response.GroupBy(g => new { g.Name, g.UserType }))
            {
                rolePermission.Add(new ListRolesPermissionResponse
                {
                    Role = roleGroup.Key.UserType == UserType.None ? roleGroup.Key.Name : roleGroup.Key.UserType.ToString(),
                    RolePermissionGroups = roleGroup.GroupBy(g => g.ModuleGroup).Select(s => new RolePermissionGroupReponse
                    {
                        ModuleGroup = s.First().ModuleGroup,
                        ModuleAction = s.Select(s => s.ModuleAction).ToArray()
                    }).ToList()
                });
            }

            return rolePermission;
        }
    }

    public class ListRolesPermissionQuery : IRequest<List<ListRolesPermissionResponse>> { }

    public class ListRolesPermissionResponse
    {
        public string Role { get; set; }
        public List<RolePermissionGroupReponse> RolePermissionGroups { get; set; }

    }
    public class RolePermissionGroupReponse
    {
        public ModuleGroup ModuleGroup { get; set; }
        public ModuleAction[] ModuleAction { get; set; }
    }
}
