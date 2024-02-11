using Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> ActivateAsync(User dbUser, string token, string password);
        Task<bool> ResetPasswordAsync(User dbUser, string token, string password);
        Task<bool> CheckPasswordAsync(User dbUser, string password);
        Task<bool> CreateAsync(User dbUser, string roleId);
        Task<bool> CreateAsync(User dbUser);
        Task<bool> DeleteAsync(User dbUser);
        Task<bool> ReconfirmUserEmailAsync(string userId, string clientUrl, string lastUpdatedBy);
        Task<string> GenerateEmailConfirmationTokenAsync(User dbUser);
        Task<string> GenerateResetPasswordTokenAsync(User dbUser);
        Task<string> GenerateTokenAsync(User dbUser);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User> GetByIdAsync(string id);
        Task<User> GetByNameAsync(string username, CancellationToken cancellationToken);
        Task<User> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<IList<string>> GetUserRolesAsync(User dbUser);
        Task<List<Role>> ListRolesAsync(CancellationToken cancellationToken);
        Task<bool> UpdateAsync(User dbUser);
    }
}
