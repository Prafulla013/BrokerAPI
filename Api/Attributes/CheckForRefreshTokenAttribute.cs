using Common.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Attributes
{
    public class CheckForRefreshTokenAttribute : TypeFilterAttribute
    {
        public CheckForRefreshTokenAttribute() : base(typeof(CheckForRefreshTokenFilter)) { }
    }
    public class CheckForRefreshTokenFilter : IAuthorizationFilter
    {
        private readonly JwtConfiguration _jwtConfiguration;
        public CheckForRefreshTokenFilter(IOptions<JwtConfiguration> jwtOptions)
        {
            _jwtConfiguration = jwtOptions.Value;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var authKey = context.HttpContext.Request.Headers["Authorization"].ToString();
                authKey = authKey.Replace("Bearer ", string.Empty);
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(authKey);
                if (token.ValidTo.Subtract(DateTime.UtcNow).Minutes < _jwtConfiguration.RefreshTimeInMinutes)
                {
                    context.HttpContext.Response.Headers.Add("must-refresh", "true");
                }
            }
        }
    }
}