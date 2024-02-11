using Application.ActivityLogs.Queries;
using Application.Employees.Queries;
using Application.Property.Commands;
using Application.ProperyManagement.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers.ClientArea
{
    public class PropertyyManagementController : BaseController
    {

        [Produces("application/json")]
        [ProducesResponseType(typeof(List<ShowCasePropertyManagementResponse>), 200)]
        [HttpGet("show-case")]
        public async Task<IActionResult> ShowCase([FromBody] ShowCasePropertyManagementQuery query)
        {

            var response = await Mediator.Send(query);
            return Ok(response);
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(List<CreateProperyManagementResponse>), 200)]
        [HttpPost("property")]
        public async Task<IActionResult> Create([FromBody] CreateProperyManagementCommand query)
        {
            query.BrokerId = CurrentBrokerId.Value;
            var response = await Mediator.Send(query);
            return Ok(response);
        }
    }
}
