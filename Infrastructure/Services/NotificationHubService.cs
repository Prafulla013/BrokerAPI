using Application.Common.Interfaces;
using Common.Enumerations;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Hubs
{
    public class NotificationHubService : Hub, INotificationHubService
    {
        private static readonly List<KeyValuePair<string, string>> ConnectedUsers = new List<KeyValuePair<string, string>>();

        private IHubContext<NotificationHubService> _hubContext;
        private IModuleActionPermissions _moduleActionPermissions;
        public NotificationHubService(IHubContext<NotificationHubService> hubContext, IModuleActionPermissions moduleActionPermissions)
        {
            _hubContext = hubContext;
            _moduleActionPermissions = moduleActionPermissions;
        }

        public override async Task OnConnectedAsync()
        {
            var role = Context.GetHttpContext().Request.Query["role"];
            var actort = Context.GetHttpContext().Request.Query["actort"];

            if (!string.IsNullOrEmpty(role))
            {
                var connectedId = Context.ConnectionId;
                ConnectedUsers.Add(new KeyValuePair<string, string>(role == "Broker Employee" ? actort : role, connectedId));
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var removeConnectedId = Context.ConnectionId;
            var userToRemove = ConnectedUsers.FirstOrDefault(user => user.Value == removeConnectedId);

            ConnectedUsers.Remove(userToRemove);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task OnRolePermissionChange(string role, UserType employeeType, CancellationToken cancellationToken)
        {
            var usersWithMatchingRole = new List<string>();
            var moduleActionPermissions = await _moduleActionPermissions.GetModuleActionPermissionsAsync(new List<string> { role }, new List<UserType> { employeeType }, cancellationToken);

            if (role == "Broker Employee")
            {
                usersWithMatchingRole = ConnectedUsers.Where(w => w.Key.Contains(((int)employeeType).ToString())).Select(s => s.Value).ToList();
            }
            else
            {
                usersWithMatchingRole = ConnectedUsers.Where(w => w.Key == role).Select(s => s.Value).ToList();
            }

            // Send the notification only to users with matching roles
            foreach (var userId in usersWithMatchingRole)
            {
                if (_hubContext.Clients.Client(userId) != null)
                {
                    await _hubContext.Clients.Client(userId).SendAsync("ReceiveNotification", moduleActionPermissions);
                }
            }

        }
    }
}
