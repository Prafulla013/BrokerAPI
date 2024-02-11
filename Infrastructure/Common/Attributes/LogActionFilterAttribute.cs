using Application.Common.Interfaces;
using Application.Common.Models;
using Common.Enumerations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Common.Attributes
{
    public class LogActionFilterAttribute : TypeFilterAttribute
    {
        public LogActionFilterAttribute() : base(typeof(LogActionFilterFilter)) { }
    }

    public class LogActionFilterFilter : IAsyncActionFilter
    {
        private readonly IUserActivityService _userActivityLog;
        private readonly IIdentityService _identityService;
        public LogActionFilterFilter(IUserActivityService userActivityLog, IIdentityService identityService)
        {
            _userActivityLog = userActivityLog;
            _identityService = identityService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            try
            {
                await OnActionExecuted(resultContext, context.ActionArguments);
            }
            catch (Exception)
            {
            }
        }

        private async Task OnActionExecuted(ActionExecutedContext context, IDictionary<string, object> dictionary)
        {
            var userId = "";
            var descriptor = ((ControllerActionDescriptor)context.ActionDescriptor);
            var controller = descriptor.ControllerName;
            var action = descriptor.ActionName;
            var moduleGroup = GetModuleGroupAction.GetModule(controller);
            var moduleAction = GetModuleGroupAction.GetAction(action);

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                userId = context.HttpContext.User.Claims.FirstOrDefault(fd => fd.Type == ClaimTypes.NameIdentifier).Value;
            }
            else if (moduleGroup == ModuleGroup.Account)
            {
                var username = "";
                var loginuserid = "";
                var email = "";
                foreach (var key in dictionary.Keys)
                {
                    var val = dictionary[key];

                    if (val is object)
                    {

                        username = (string)val.GetType().GetProperty("Username")?.GetValue(val);
                        loginuserid = (string)val.GetType().GetProperty("UserId")?.GetValue(val);
                        email = (string)val.GetType().GetProperty("Email")?.GetValue(val);
                    }
                }
                if (username != null)
                {
                    var dbUser = await _identityService.GetByNameAsync(username, CancellationToken.None);
                    if(dbUser != null)
                    {
                        userId = dbUser.Id;
                    }
                }
                else if (loginuserid != null)
                {
                    userId = loginuserid;
                }
                else
                {
                    var dbUser = await _identityService.GetByEmailAsync(email, CancellationToken.None);
                    if (dbUser != null)
                    {
                        userId = dbUser.Id;
                    }
                }
            }
            if (userId != null)
            {
                var resultResponse = (context.Result as ObjectResult);

                var Comment = context.Exception != null ? $"{context.Exception?.Message} {context.Exception?.InnerException}" : resultResponse.Value.ToString();

                var userActivity = new UserActivityModel
                {
                    UserId = userId,
                    Url = context.HttpContext.Request.GetDisplayUrl(),
                    Comment = resultResponse != null && (HttpStatusCode)resultResponse.StatusCode == HttpStatusCode.OK ? "successfully executed" : Comment,
                    ModuleAction = moduleAction,
                    ModuleGroup = moduleGroup,
                    StatusCode = resultResponse != null ? (HttpStatusCode)resultResponse?.StatusCode : HttpStatusCode.BadRequest,
                };
                await _userActivityLog.AddUserActivityLog(userActivity, CancellationToken.None);
            }
            
        }
    }
}
