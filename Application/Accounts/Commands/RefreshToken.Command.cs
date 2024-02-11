using Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts.Commands
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IIdentityService _identityService;
        public RefreshTokenHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var dbUser = await _identityService.GetByUserIdAsync(request.UserId, cancellationToken);
            var token = await _identityService.GenerateTokenAsync(dbUser);

            var response = new RefreshTokenResponse
            {
                RefreshToken = token
            };
            return response;
        }
    }

    public class RefreshTokenCommand : IRequest<RefreshTokenResponse>
    {
        public string UserId { get; set; }
    }

    public class RefreshTokenResponse
    { 
        public string RefreshToken { get; set; }
    }
}