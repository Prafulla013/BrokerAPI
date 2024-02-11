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
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        public ResetPasswordHandler(IBrokerDbContext dbContext,
                                    IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            
            var dbUser = await _identityService.GetByEmailAsync(request.Email, cancellationToken);
            if (dbUser == null)
            {
                throw new BadRequestException("Invalid user request.");
            }

            dbUser.ClientUrl = request.ClientUrl;
            dbUser.LastUpdatedBy = $"{dbUser.Profile.FirstName} {dbUser.Profile.LastName}";
            dbUser.Activity = ActivityLog.PasswordReset;

            await _identityService.ResetPasswordAsync(dbUser, request.Token, request.Password);

            return new ResetPasswordResponse();
        }
    }

    public class ResetPasswordCommand : IRequest<ResetPasswordResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        [JsonIgnore]
        public string Subdomain { get; set; }
        [JsonIgnore]
        public string ClientUrl { get; set; }
    }

    public class ResetPasswordResponse { }
}
