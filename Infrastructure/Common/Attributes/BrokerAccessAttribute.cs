using Application.Common.Interfaces;
using Common.Enumerations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Common.Attributes
{
    public class BrokerAccessAttribute : TypeFilterAttribute
    {
        public BrokerAccessAttribute() : base(typeof(BrokerAccessFilter)) { }
    }

    public class BrokerAccessFilter : IAsyncAuthorizationFilter
    {
        private readonly IPermissionService _permissionService;
        public BrokerAccessFilter(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string subdomain = context.HttpContext.Request.Headers["subdomain"];
                // subdomain for root always should be admin
                if (string.IsNullOrEmpty(subdomain) || subdomain == "admin")
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var broker = context.HttpContext.User?.Claims.FirstOrDefault(fd => fd.Type == ClaimTypes.System)?.Value;
                if (!long.TryParse(broker, out long brokerId))
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var userId = context.HttpContext.User.Claims.FirstOrDefault(fd => fd.Type == ClaimTypes.NameIdentifier).Value;
                var roles = context.HttpContext.User.Claims.Where(w => w.Type == ClaimTypes.Role).Select(s => s.Value).ToArray();
                UserType[] employeeTypes = context.HttpContext.User.Claims.Where(w => w.Type == ClaimTypes.Actor)
                                                                              .Select(s => (UserType)int.Parse(s.Value)).ToArray();

                var descriptor = ((ControllerActionDescriptor)context.ActionDescriptor);
                var controller = descriptor.ControllerName;
                var action = descriptor.ActionName;

                var moduleGroup = GetModuleGroupAction.GetModule(controller);
                if (moduleGroup == 0)
                {
                    context.Result = new ForbidResult();
                    return;
                }
                var moduleAction = GetModuleGroupAction.GetAction(action);
                if (moduleAction == 0)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                var hasPermission = await _permissionService.HasPermissionAsync(brokerId, subdomain, userId, roles, employeeTypes, moduleGroup, moduleAction, CancellationToken.None);
                if (!hasPermission)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}
