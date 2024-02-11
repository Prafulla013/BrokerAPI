using Application.Common.Interfaces;
using Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class ReinviteUserHandler : IRequestHandler<ReinviteUserCommand, ReinviteUserResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IStringHelper _stringHelper;
        public ReinviteUserHandler(IBrokerDbContext dbContext,
                                   IIdentityService identityService,
                                   IStringHelper stringHelper)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _stringHelper = stringHelper;
        }

        public async Task<ReinviteUserResponse> Handle(ReinviteUserCommand request, CancellationToken cancellationToken)
        {
            string subdomain = "";

            var dbBroker = await _dbContext.Brokers.FindAsync(request.BrokerId);

            var dbProfile = await _dbContext.UserProfiles.AsNoTracking()
                                                         .FirstOrDefaultAsync(fd => fd.Id == request.Id, cancellationToken);

            if (dbProfile == null)
            {
                throw new NotFoundException("Invalid user request.");
            }
            else if (dbProfile.BrokerId != request.BrokerId)
            {
                throw new BadRequestException("Invalid user request.");
            }

            await _identityService.ReconfirmUserEmailAsync(dbProfile.Id, _stringHelper.ToClientUrl(subdomain), request.CurrentUser);

            return new ReinviteUserResponse();
        }
    }

    public class ReinviteUserCommand : IRequest<ReinviteUserResponse>
    {
        public string Id { get; set; }
        public long? BrokerId { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
    }

    public class ReinviteUserResponse { }
}
