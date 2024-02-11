using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class RequestResetPasswordHandler : IRequestHandler<RequestResetPasswordCommand, RequestResetPasswordResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        public RequestResetPasswordHandler(IBrokerDbContext dbContext,
                                           IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public async Task<RequestResetPasswordResponse> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
        {
           
            var dbUser = await _identityService.GetByEmailAsync(request.Email, cancellationToken);
            if (dbUser == null)
            {
                throw new BadRequestException("Invalid user request.");
            }

            dbUser.ClientUrl = request.ClientUrl;
            dbUser.LastUpdatedBy = $"{dbUser.Profile.FirstName} {dbUser.Profile.LastName}";
            dbUser.Activity = ActivityLog.RequestPasswordReset;

            await _identityService.UpdateAsync(dbUser);

            // TODO : the event is not dispacted is response is not returned.
            return new RequestResetPasswordResponse();
        }
    }

    public class RequestResetPasswordCommand : IRequest<RequestResetPasswordResponse>
    {
        public string Email { get; set; }
        [JsonIgnore]
        public string Subdomain { get; set; }
        [JsonIgnore]
        public string ClientUrl { get; set; }
    }

    public class RequestResetPasswordResponse { }
}
