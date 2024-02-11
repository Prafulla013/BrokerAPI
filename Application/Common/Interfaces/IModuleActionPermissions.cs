using Application.Accounts.Commands;
using Common.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IModuleActionPermissions
    {
        Task<List<ListModulePermissionResponse>> GetModuleActionPermissionsAsync(IList<string> roles, List<UserType> employeeTypes, CancellationToken cancellationToken);
    }
}
