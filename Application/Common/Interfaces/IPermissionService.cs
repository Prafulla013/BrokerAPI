using Common.Enumerations;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> IsValidBroker(long brokerId, string subdomain, CancellationToken cancellationToken);
        Task<bool> HasPermissionAsync(string userId, string[] roles, ModuleGroup moduleGroup, ModuleAction moduleAction, CancellationToken cancellationToken);
        Task<bool> HasPermissionAsync(long brokerId, string subdomain, string userId, string[] roles, UserType[] employeeTypes, ModuleGroup moduleGroup, ModuleAction moduleAction, CancellationToken cancellationToken);
    }
}
