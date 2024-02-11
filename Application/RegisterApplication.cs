using Application.Accounts.Services;
using Application.Common.Helper;
using Application.Common.Interfaces;
using Application.Common.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class RegisterApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterApplication).Assembly));

            services.AddValidatorsFromAssembly(typeof(RegisterApplication).Assembly);

            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<IStringHelper, StringHelper>();
            services.AddScoped<IModuleActionPermissions, ModuleActionPermissions>();

            return services;
        }
    }
}
