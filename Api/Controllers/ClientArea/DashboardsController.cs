using Application.ActivityLogs.Queries;
using Application.Employees.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers.ClientArea
{
    public class DashboardsController : BaseController
    {
         
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<ListActivityLogResponse>), 200)]
        [HttpGet("activity-logs")]
        public async Task<IActionResult> ActivityLogs()
        {
            var query = new ListActivityLogQuery
            {
                BrokerId = CurrentBrokerId.Value
            };

            var response = await Mediator.Send(query);
            return Ok(response);
        }
    }
}
