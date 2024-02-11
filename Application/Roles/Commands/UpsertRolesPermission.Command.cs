using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Roles.Commands
{
    public class UpsertRolesPermissionHandler : IRequestHandler<UpsertRolesPermissionCommand, bool>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly INotificationHubService _notificationHub;
        public UpsertRolesPermissionHandler(IBrokerDbContext dbContext, IIdentityService identityService, INotificationHubService notificationHub)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _notificationHub = notificationHub;

        }

        public async Task<bool> Handle(UpsertRolesPermissionCommand request, CancellationToken cancellationToken)
        {

            var dbRole = await _identityService.GetRoleByNameAsync(request.RoleName);
            if (dbRole == null)
            {
                throw new NotFoundException("Invalid role name.");
            }

            var dbActionRoles = await _dbContext.ModuleActionRolePermissions.Where(w => w.RoleId == dbRole.Id &&
                                                                                       w.UserType == request.UserType &&
                                                                                       w.ModuleGroup == request.ModuleGroupId)
                                                                           .ToListAsync(cancellationToken);

            if (!dbActionRoles.Any(a => a.ModuleAction == request.ModuleActionId))
            {
                _dbContext.ModuleActionRolePermissions.Add(new ModuleActionRolePermission
                {
                    ModuleGroup = request.ModuleGroupId,
                    ModuleAction = request.ModuleActionId,
                    IsActive = request.IsSelected,
                    RoleId = dbRole.Id,
                    UserType = request.UserType,
                    LastUpdatedBy = request.CurrentUser,
                    LastUpdateDateTime = DateTimeOffset.UtcNow
                });
            }
            else
            {
                var dbActionsToUpdate = dbActionRoles.FirstOrDefault(w => w.ModuleAction == request.ModuleActionId);
                dbActionsToUpdate.IsActive = request.IsSelected;
            }

            if (request.ModuleActionId == ModuleAction.Read && !request.IsSelected)
            {
                foreach (var role in dbActionRoles)
                {
                    role.IsActive = request.IsSelected;
                }
            }
            else if (request.ModuleActionId != ModuleAction.Read)
            {
                var readRole = dbActionRoles.FirstOrDefault(w => w.ModuleAction == ModuleAction.Read);

                if (readRole != null && request.IsSelected)
                {
                    readRole.IsActive = request.IsSelected;
                }
                else if (request.IsSelected)
                {
                    _dbContext.ModuleActionRolePermissions.Add(new ModuleActionRolePermission
                    {
                        ModuleGroup = request.ModuleGroupId,
                        ModuleAction = ModuleAction.Read,
                        IsActive = request.IsSelected,
                        RoleId = dbRole.Id,
                        UserType = request.UserType,
                        LastUpdatedBy = request.CurrentUser,
                        LastUpdateDateTime = DateTimeOffset.UtcNow
                    });
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _notificationHub.OnRolePermissionChange(request.RoleName, request.UserType, cancellationToken);
            
            return true;
        }
    }

    public class UpsertRolesPermissionCommand : IRequest<bool>
    {
        [JsonIgnore]
        public string RoleName { get; set; }
        public UserType UserType { get; set; }
        public ModuleGroup ModuleGroupId { get; set; }
        public ModuleAction ModuleActionId { get; set; }
        public bool IsSelected { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
    }
}
