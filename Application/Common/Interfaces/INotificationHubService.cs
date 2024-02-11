using Common.Enumerations;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface INotificationHubService
    {
        Task OnRolePermissionChange(string role, UserType employeeType, CancellationToken cancellationToken);
    }
}
