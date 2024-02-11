using Application.Employees.Commands;
using Application.Employees.Queries;
using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers.ClientArea
{
    public class EmployeesController : BaseController
    {
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<ListEmployeesResponse>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListEmployeesQuery query)
        {
            try
            {
                query.BrokerId = CurrentBrokerId.Value;

                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(GetEmployeeByIdResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            try
            {
                var query = new GetEmployeeByIdQuery
                {
                    Id = id,
                    BrokerId = CurrentBrokerId.Value
                };

                var response = await Mediator.Send(query);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(CreateEmployeeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            try
            {
                command.CurrentUser = CurrentUsername;
                command.Subdomain = Subdomain;
                command.BrokerId = CurrentBrokerId.Value;

                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(UpdateEmployeeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateEmployeeCommand command)
        {
            try
            {
                command.Id = id;
                command.CurrentUser = CurrentUsername;
                command.Subdomain = Subdomain;
                command.BrokerId = CurrentBrokerId.Value;

                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(DeleteEmployeeResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                var command = new DeleteEmployeeCommand
                {
                    Id = id,
                    BrokerId = CurrentBrokerId.Value
                };
                var response = await Mediator.Send(command);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest("Cannot delete employee.");
            }
        }
    }
}
