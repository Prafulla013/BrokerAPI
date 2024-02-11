using Api.Attributes;
using Common.Configurations;
using Common.Enumerations;
using Infrastructure.Common.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;

namespace Api.Controllers.ClientArea
{
    [Route("clientarea/[controller]")]
    [ApiController]
    [BrokerAccess]
    [CheckForRefreshToken]
    public abstract class BaseController : ControllerBase
    {
        private readonly ApplicationConfiguration _applicationConfiguration;
        public BaseController(IOptions<ApplicationConfiguration> options = default)
        {
            _applicationConfiguration = options?.Value;
        }

        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        protected string CurrentUserId => HttpContext.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier)?.Value;
        protected string CurrentUsername => HttpContext.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name)?.Value ?? "SA";
        protected string Role => HttpContext.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Role)?.Value;
        protected long? CurrentBrokerId => long.TryParse(HttpContext.User?.Claims.FirstOrDefault(f => f.Type == ClaimTypes.System)?.Value, out long brokerId) ? brokerId : null;
        protected string Subdomain => HttpContext.Request?.Headers["subdomain"];

        protected string ClientUrl
        {
            get
            {
                var subdomain = "";
                if (!string.IsNullOrEmpty(Subdomain))
                {
                    subdomain = $"{Subdomain}.";
                }

                return $"{_applicationConfiguration.Protocol}{subdomain}{_applicationConfiguration.ClientUrl}";
            }
        }
    }
}
