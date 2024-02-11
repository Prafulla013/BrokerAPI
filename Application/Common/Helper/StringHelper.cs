using Application.Common.Interfaces;
using Common.Configurations;
using Microsoft.Extensions.Options;

namespace Application.Common.Helper
{
    public sealed class StringHelper : IStringHelper
    {
        private readonly ApplicationConfiguration _appConfig;
        public StringHelper(IOptions<ApplicationConfiguration> options)
        {
            _appConfig = options.Value;
        }

        public string ToClientUrl(string subdomain)
        {
            if (!string.IsNullOrEmpty(subdomain))
            {
                subdomain = $"{subdomain}.";
            }

            return $"{_appConfig.Protocol}{subdomain}{_appConfig.ClientUrl}";
        }
    }
}
