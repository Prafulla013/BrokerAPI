using Application.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IUserActivityService
    {
        Task<bool> AddUserActivityLog(UserActivityModel userActivity, CancellationToken cancellationToken);
    }
}
