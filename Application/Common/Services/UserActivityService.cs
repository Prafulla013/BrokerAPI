using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly IBrokerDbContext _dbContext;
        public UserActivityService(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddUserActivityLog(UserActivityModel userActivity,
                                                    CancellationToken cancellationToken)
        {
            var dbUserActivityLog = new UserActivityLog
            {
                UserId = userActivity.UserId,
                Url = userActivity.Url,
                Comment = userActivity.Comment,
                ModuleAction = userActivity.ModuleAction,
                ModuleGroup = userActivity.ModuleGroup,
                StatusCode = userActivity.StatusCode,
                IsActive = true
            };
            _dbContext.UserActivityLogs.Add(dbUserActivityLog);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
